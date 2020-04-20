using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ListaPesama_SignalRClient.Models;
using ListaPesama_SignalRClient.Services;

namespace ListaPesama_SignalRClient.Activities
{
    [Activity(Label = "Song Details")]
    public class SongDetailsActivity : Activity
    {
        ISongService songService;
        Button btnSave, btnCancel, btnDelete;
        EditText etGenre, etTitle, etOrder, etArtist;
        TextView tvOrder;
        string songId;

        public SongDetailsActivity() : this(new SongService())
        { }

        public SongDetailsActivity(ISongService repository)
        {
            songService = repository;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SongDetails);

            SetControls();

            songId = Intent.GetStringExtra("SelectedId");

            if (songId != null)
            {
                FillControls(songId);
            }
            else
            {
                btnDelete.Visibility = ViewStates.Gone;
                etOrder.Visibility = ViewStates.Gone;
                tvOrder.Visibility = ViewStates.Gone;
            }

            btnSave.Click += btnSave_OnClick;
            btnCancel.Click += btnCancel_OnClick;
            btnDelete.Click += BtnDelete_Click;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            songService.DeleteSong(int.Parse(songId));
            StartActivity(typeof(MainActivity));
        }

        private void btnCancel_OnClick(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }

        private void btnSave_OnClick(object sender, EventArgs e)
        {
            Song newSong = new Song
            {
                Genre = etGenre.Text,
                Title = etTitle.Text,
                Artist = etArtist.Text,
                OrderNumber = songId != null ? int.Parse(etOrder.Text) : 0
            };

            if (songId != null)
            {
                newSong.ID = int.Parse(songId);
                songService.UpdateSong(newSong);
            }
            else
            {
                songService.AddSong(newSong);
            }

            StartActivity(typeof(MainActivity));
        }

        private void FillControls(string songId)
        {
            Song selectedSong = new Song();
            if (songId != null)
            {
                selectedSong = songService.GetSong(int.Parse(songId));
            }

            etGenre.Text = selectedSong.Genre;
            etTitle.Text = selectedSong.Title;
            etTitle.Text = selectedSong.Artist;
            etOrder.Text = selectedSong.OrderNumber.ToString();
        }

        private void SetControls()
        {
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnCancel = FindViewById<Button>(Resource.Id.btnCancel);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            etGenre = FindViewById<EditText>(Resource.Id.etGenre);
            etTitle = FindViewById<EditText>(Resource.Id.etTitle);
            etArtist = FindViewById<EditText>(Resource.Id.etArtist);
            etOrder = FindViewById<EditText>(Resource.Id.etOrder);
            tvOrder = FindViewById<TextView>(Resource.Id.tvOrder);
        }
    }
}