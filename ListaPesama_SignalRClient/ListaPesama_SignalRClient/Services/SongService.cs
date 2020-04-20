using System.Collections.Generic;
using System.IO;
using System.Linq;
using ListaPesama_SignalRClient.Models;
using SQLite;

namespace ListaPesama_SignalRClient.Services
{
    public class SongService : ISongService
    {
        SQLiteConnection db;
        object locker = new object();

        public SongService()
        {
            string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "songs.db3");
            db = new SQLiteConnection(dbPath);
            db.CreateTable<Song>();
        }

        public void ImportSongs(List<Song> songList)
        {
            db.DeleteAll<Song>();
            foreach (var song in songList)
            {
                db.Insert(song);
            }
        }

        public List<Song> GetSongList()
        {
            lock (locker)
            {
                return db.Table<Song>().OrderBy(x => x.OrderNumber).ToList();
            }
        }

        public Song GetSong(int id)
        {
            return db.Get<Song>(id);
        }

        public void AddSong(Song song)
        {
            song.OrderNumber = db.Table<Song>().Count() != 0 ? db.Table<Song>().Max(x => x.OrderNumber) + 1 : 1;
            db.Insert(song);
        }

        public void UpdateSong(Song song)
        {
            var currentOrderNumber = db.Get<Song>(song.ID).OrderNumber;
            var maxOrderNumber = db.Table<Song>().Max(x => x.OrderNumber);
            if (song.OrderNumber == currentOrderNumber)
            {
                Update(song);
            }
            else if (song.OrderNumber > maxOrderNumber)
            {
                var lastItem = db.Table<Song>().First(x => x.OrderNumber == maxOrderNumber);
                lastItem.OrderNumber = currentOrderNumber;
                song.OrderNumber = maxOrderNumber;
                Update(song);
                Update(lastItem);
            }
            else
            {
                var itemToChageOrder = db.Table<Song>().FirstOrDefault(x => x.OrderNumber == song.OrderNumber);
                if (itemToChageOrder != null)
                {
                    itemToChageOrder.OrderNumber = currentOrderNumber;
                    Update(itemToChageOrder);
                }
                Update(song);
            }
        }

        private void Update(Song song)
        {
            db.Update(song);
        }

        public List<Song> SearchSongs(string search)
        {
            List<Song> filterSongs = db.Table<Song>().Where(x => x.Title.Contains(search) || x.Artist.Contains(search)).OrderBy(x => x.OrderNumber).ToList();
            return filterSongs;
        }

        public void DeleteSong(int id)
        {
            db.Delete<Song>(id);
        }
    }
}