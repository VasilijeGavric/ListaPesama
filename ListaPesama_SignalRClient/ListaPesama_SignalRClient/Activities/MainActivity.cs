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

namespace ListaPesama_SignalRClient.Activities
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

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
            var serverName = listaPesamaPreferences.GetString("ServerName", null);

            var hubConnection = new HubConnection($"http://{serverName}"); //"http://192.168.0.11:555"; "http://mysignalrsample.azurewebsites.net/"
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            try
            {
                await hubConnection.Start();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.InnerException.Message, ToastLength.Short).Show();
            }

            return chatHubProxy;
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