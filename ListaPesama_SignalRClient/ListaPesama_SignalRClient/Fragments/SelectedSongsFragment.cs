using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ListaPesama_SignalRClient.Services;
using Microsoft.AspNet.SignalR.Client;

namespace ListaPesama_SignalRClient.Fragments
{
    public class SelectedSongsFragment : Fragment
    {
        ArrayAdapter lvSelectedSongsAdapter;
        ListView lvSelectedSongs;
        Button btnClearSelectedList;
        ISelectedSongsService _selectedSongsService;
        IDisposable chatHubProxyDisposable;
        IHubProxy _hubProxy;

        public SelectedSongsFragment(ISelectedSongsService selectedSongsService, IHubProxy hubProxy)
        {
            _selectedSongsService = selectedSongsService;
            _hubProxy = hubProxy;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            chatHubProxyDisposable = _hubProxy.On<string>("UpdateMessage", (message) =>
            {
                //UpdateChatMessage has been called from Sever
                try
                {
                    Activity.RunOnUiThread(() =>
                    {
                        _selectedSongsService.AddSong(message);
                        //todo: change refresh list
                        lvSelectedSongsAdapter.Clear();
                        lvSelectedSongsAdapter.AddAll(_selectedSongsService.GetSelectedSongs());
                    });
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity, ex.InnerException.Message, ToastLength.Short).Show();
                }
            });
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.SelectedSongsList, container, false);
            lvSelectedSongs = view.FindViewById<ListView>(Resource.Id.lvSelectedSongs);
            btnClearSelectedList = view.FindViewById<Button>(Resource.Id.btnClearSelectedList);
            btnClearSelectedList.Click += BtnClearSelectedList_Click;

            lvSelectedSongsAdapter = new ArrayAdapter<string>(Activity, Android.Resource.Layout.SimpleListItem1, _selectedSongsService.GetSelectedSongs());
            lvSelectedSongs.Adapter = lvSelectedSongsAdapter;

            return view;
        }

        private void BtnClearSelectedList_Click(object sender, EventArgs e)
        {
            _selectedSongsService.ClearList();
            lvSelectedSongsAdapter.Clear();
        }

        public override void OnStop()
        {
            base.OnStop();
            chatHubProxyDisposable.Dispose();
        }
    }
}