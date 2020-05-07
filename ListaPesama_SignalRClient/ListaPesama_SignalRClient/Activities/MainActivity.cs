using Android.App;
using Android.OS;
using ListaPesama_SignalRClient.Services;
using Android.Views;
using Android.Content;
using ListaPesama_SignalRClient.Fragments;
using Microsoft.AspNet.SignalR.Client;
using System;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using Plugin.Connectivity;

namespace ListaPesama_SignalRClient.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);

            var selectedSongsService = new SelectedSongsService();
            var songService = new SongService();
            IHubProxy chatHubProxy = await CreateHubProxy();

            var allSongsFragment = new AllSongsFragment(songService, selectedSongsService, chatHubProxy);
            var selectedSongsFragment = new SelectedSongsFragment(selectedSongsService, chatHubProxy);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            ActionBar.Tab tabAllSongs = CreateTab("Sve pesme", allSongsFragment);
            ActionBar.AddTab(tabAllSongs);

            ActionBar.Tab tabSelectedSongs = CreateTab("Izabrane pesme", selectedSongsFragment);
            ActionBar.AddTab(tabSelectedSongs);

            tabAllSongs.Select();
        }

        private async Task<IHubProxy> CreateHubProxy()
        {
            var listaPesamaPreferences = Application.Context.GetSharedPreferences("ListaPesama", FileCreationMode.Private);
            string serverName = listaPesamaPreferences.GetString("ServerName", null);

            bool hostIsAvailable = await IsServiceAvailable(serverName);

            if (!hostIsAvailable)
            {
                string ipAdress = GetIpAddress();

                for (int i = 100; i < 105; i++)
                {
                    hostIsAvailable = await IsServiceAvailable(ipAdress + i.ToString());
                    if (hostIsAvailable)
                    {
                        serverName = ipAdress + i.ToString();
                        break;
                    }
                }
            }

            if (hostIsAvailable)
            {
                var preferencesEditor = listaPesamaPreferences.Edit();
                preferencesEditor.PutString("ServerName", serverName);
                preferencesEditor.Commit();
            }

            var hubConnection = new HubConnection($"http://{serverName}:555"); //"http://192.168.0.11:555"; "http://mysignalrsample.azurewebsites.net/"
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            try
            {
                await hubConnection.Start();
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Greska prilikom rada aplikacije. Proverite wireless konekcije.", ToastLength.Short).Show();
            }

            return chatHubProxy;
        }

        public async Task<bool> IsServiceAvailable(string serverName)
        {
            var connectivity = CrossConnectivity.Current;
            if (!connectivity.IsConnected)
                return false;

            var reachable = await connectivity.IsRemoteReachable(serverName, 555);

            return reachable;
        }

        private string GetIpAddress()
        {
            IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            string ipAddress = string.Empty;
            if (addresses != null && addresses[0] != null)
            {
                ipAddress = addresses[0].ToString();
            }
            else
            {
                ipAddress = null;
            }

            return ipAddress.Substring(0,ipAddress.LastIndexOf(".") + 1);
        }

        private ActionBar.Tab CreateTab(string title, Fragment fragment)
        {
            ActionBar.Tab newTab = ActionBar.NewTab();
            newTab.SetText(title);
            newTab.TabSelected += (sender, args) =>
            {
                args.FragmentTransaction.Replace(Resource.Id.fragmentContainer, fragment);
            };

            return newTab;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            var inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.option_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if(item.ItemId == Resource.Id.menu_addSong)
            {
                var addSongActivity = new Intent(this, typeof(SongDetailsActivity));
                StartActivity(addSongActivity);
                return true;
            }
            else if (item.ItemId == Resource.Id.menu_openPreferences)
            {
                var addSongActivity = new Intent(this, typeof(PreferencesActivity));
                StartActivity(addSongActivity);
                return true;
            }
            else if (item.ItemId == Resource.Id.menu_exit)
            {
                Java.Lang.JavaSystem.Exit(0);
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}