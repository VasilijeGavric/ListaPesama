using Android.App;
using Android.Views;
using Android.Widget;
using ListaPesama_SignalRClient.Models;

namespace ListaPesama_SignalRClient.Utils
{
    public class SongsAdapter : BaseAdapter<Song>
    {
        Song[] songs;
        Activity context;
        public SongsAdapter(Activity context, Song[] songs) : base()
        {
            this.context = context;
            this.songs = songs;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override Song this[int position]
        {
            get { return songs[position]; }
        }
        public override int Count
        {
            get { return songs.Length; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView; // re-use an existing view, if one is available
            if (view == null) // otherwise create a new one
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = string.IsNullOrEmpty(songs[position].Artist) ? songs[position].Title : $"{songs[position].Artist} - {songs[position].Title}";
            return view;
        }
    }
}