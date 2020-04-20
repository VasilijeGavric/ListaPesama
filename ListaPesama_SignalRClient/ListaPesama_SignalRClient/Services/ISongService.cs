using System.Collections.Generic;
using ListaPesama_SignalRClient.Models;

namespace ListaPesama_SignalRClient.Services
{
    public interface ISongService
    {
        void AddSong(Song song);
        void DeleteSong(int id);
        Song GetSong(int id);
        List<Song> GetSongList();
        List<Song> SearchSongs(string search);
        void UpdateSong(Song song);
        void ImportSongs(List<Song> songList);
    }
}