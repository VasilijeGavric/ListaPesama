using System.Collections.Generic;

namespace ListaPesama_SignalRClient.Services
{
    public interface ISelectedSongsService
    {
        void AddSong(string songName);
        void ClearList();
        List<string> GetSelectedSongs();
    }
}