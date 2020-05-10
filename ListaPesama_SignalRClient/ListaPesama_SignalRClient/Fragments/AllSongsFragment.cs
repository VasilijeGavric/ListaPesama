using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using ListaPesama_SignalRClient.Activities;
using ListaPesama_SignalRClient.Models;
using ListaPesama_SignalRClient.Services;
using ListaPesama_SignalRClient.Utils;
using Microsoft.AspNet.SignalR.Client;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;

namespace ListaPesama_SignalRClient.Fragments
{
    public class AllSongsFragment : Fragment
    {
        ListView lvSongs;
        protected SongsAdapter songsAdapter;
        ISongService _songService;
        ISelectedSongsService _selectedSongsService;
        View view;
        EditText etSearch;
        Button btnOpenFile;
        CancellationTokenSource cancellationTokenSource;
        IDisposable chatHubProxyDisposable;
        IHubProxy _hubProxy;

        public AllSongsFragment(ISongService songService, ISelectedSongsService selectedSongsService, IHubProxy hubProxy)
        {
            _songService = songService;
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
                        Toast.MakeText(Activity, Resource.String.successfullyAddedSong, ToastLength.Short).Show();
                    });
                }
                catch (Exception)
                {
                    Toast.MakeText(Activity, Resource.String.errorConnectivity, ToastLength.Short).Show();
                }
            });
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(Resource.Layout.AllSongsList, container, false);
            SetControls();
            return view;
        }

        private void SetControls()
        {
            lvSongs = view.FindViewById<ListView>(Resource.Id.lvSongs);
            etSearch = view.FindViewById<EditText>(Resource.Id.etSearch);
            btnOpenFile = view.FindViewById<Button>(Resource.Id.btnOpenFile);

            List<Song> songs = _songService.GetSongList();
            songsAdapter = new SongsAdapter(Activity, songs.ToArray());
            
            lvSongs.Adapter = songsAdapter;
            lvSongs.TextFilterEnabled = true;
            lvSongs.ItemLongClick += LvSongs_ItemLongClick;
            etSearch.TextChanged += EtSearch_TextChanged;
            btnOpenFile.Click += BtnOpenFile_Click;
            lvSongs.ItemClick += LvSongs_ItemClick;
        }

        private async void LvSongs_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            try
            {
                var message = ((TextView)e.View).Text;
                await _hubProxy.Invoke("SendMessage", new object[] { message });
            }
            catch (Exception)
            {
                Toast.MakeText(Activity, Resource.String.errorConnectivity, ToastLength.Short).Show();
            }
        }

        private async void BtnOpenFile_Click(object sender, EventArgs e)
        {
            int orderNumber = 0;
            List<Song> songList = new List<Song>();

            try
            {
                FileData filedata = await CrossFilePicker.Current.PickFile();

                var dataArray = filedata.DataArray;
                using (Stream stream = new MemoryStream(dataArray))
                {
                    using (CsvFileReader reader = new CsvFileReader(stream))
                    {
                        CsvRow row = new CsvRow();
                        while (reader.ReadRow(row))
                        {
                            orderNumber++;
                            Song newSong = new Song
                            {
                                Artist = row[0],
                                Title = row[1],
                                Genre = row.Count == 3 ? row[2] : "",
                                OrderNumber = orderNumber
                            };
                            songList.Add(newSong);
                        }
                    }
                }

                if (songList.Count > 0)
                {
                    _songService.ImportSongs(songList);

                    List<Song> songs = _songService.GetSongList();
                    songsAdapter = new SongsAdapter(Activity, songs.ToArray());
                }
            }
            catch (Exception)
            {
                Toast.MakeText(Activity, "Zadnja ispravno ucitana pesma je <" + songList[orderNumber - 2].Title + ">. Proverite pesmu posle nje.", ToastLength.Short).Show();
            }
        }

        private void LvSongs_ItemLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            var itemPosition = e.Position;

            Song selectedSong = lvSongs.GetItemAtPosition(itemPosition).Cast<Song>();

            var songDetailsActivity = new Intent(Activity, typeof(SongDetailsActivity));
            songDetailsActivity.PutExtra("SelectedId", selectedSong.ID.ToString());
            StartActivity(songDetailsActivity);
        }

        private async void EtSearch_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                cancellationTokenSource?.Cancel();     // cancel previous search
            }
            catch (ObjectDisposedException)     // in case previous search completed
            {
            }

            using (cancellationTokenSource = new CancellationTokenSource())
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(800), cancellationTokenSource.Token);  // buffer

                    List<Song> songs = _songService.SearchSongs(etSearch.Text);
                    songsAdapter = new SongsAdapter(Activity, songs.ToArray());

                    lvSongs.Adapter = songsAdapter;
                }
                catch (TaskCanceledException)       // if the operation is cancelled, do nothing
                {
                }
            }
        }

        public override void OnStop()
        {
            base.OnStop();
            chatHubProxyDisposable.Dispose();
        }
    }
}