using System.Collections.Generic;

namespace ListaPesama_SignalRClient.Services
{
    public class SelectedSongsService : ISelectedSongsService
    {
        private static List<string> selectedSongs;

        public void AddSong(string songName)
        {
            if (selectedSongs == null)
            {
                selectedSongs = new List<string>();
            }

            selectedSongs.Add(songName);
        }

        public void ClearList()
        {
            selectedSongs.Clear();
        }

        public List<string> GetSelectedSongs()
        {
            List<string> reverseList = new List<string>();
            if (selectedSongs != null)
            {
                reverseList = new List<string>(selectedSongs);
                reverseList.Reverse();
            }
            return reverseList;
        }
    }
}