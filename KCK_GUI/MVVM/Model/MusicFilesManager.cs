using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KCK_GUI.MVVM.Model
{
    public sealed class MusicFilesManager
    {
        private MusicFilesManager() { }

        private static MusicFilesManager _instance;

        public static MusicFilesManager GetInstance()
        {
            if (_instance == null) 
            {
                _instance = new MusicFilesManager();
            }
            return _instance;
        }

        //Logika
        private static List<Song> CurrentSongList { get; set; }
        public List<Song> getCurrentSongList() 
        {
            return CurrentSongList;
        }
        public void LoadAllMusicFiles()
        {
            
            CurrentSongList = new List<Song>();
            string path = ConfigurationManager.AppSettings["MusicFilesDirectory"];
            ProcessDirectory(path);

        }
       
         private void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory, "*.mp3");
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
        private void ProcessFile(string path)
        {
            SongDataReader songDataReader = new SongDataReader();
            CurrentSongList.Add(songDataReader.ReadData(Path.GetFileName(path)));
        }
        //Pobieranie playlisty
        public void LoadPlaylist(JsonManager jsonManager)
        {
            LoadAllMusicFiles();
            List<Song> tempSongList = CurrentSongList;
            List<Song> playList = new List<Song>();
            if (jsonManager.IsFileExisting())
            {
                playList = JsonConvert.DeserializeObject<List<Song>>(jsonManager.getJsonFile());
            }
            foreach (var song in playList) 
            {
                if (!tempSongList.Contains(song)) 
                {
                    playList.Remove(song);
                }
                
            }
            CurrentSongList = playList;
        }

        public void AddMusicToPlaylist(Song song, JsonManager jsonManager)
        {
            List<Song> playList = new List<Song>();
            if (jsonManager.IsFileExisting())
            {
                playList = JsonConvert.DeserializeObject<List<Song>>(jsonManager.getJsonFile());
            }

            if (!playList.Contains(playList.Find(p => p.Path == song.Path)))
            {
                playList.Add(song);
                string jsonString = JsonConvert.SerializeObject(playList);
                jsonManager.writeJson(jsonString);
            }
        }

        public static void DeleteMusicFromPlaylist(Song song, JsonManager jsonManager)
        {
            List<Song> playList = new List<Song>();
            if (jsonManager.IsFileExisting())
            {
                playList = JsonConvert.DeserializeObject<List<Song>>(jsonManager.getJsonFile());
            }
            
            if (playList.Contains(playList.Find(p => p.Path == song.Path)))
            {
                playList.Remove(playList.Find(p => p.Path == song.Path));
                string jsonString = JsonConvert.SerializeObject(playList);
                jsonManager.writeJson(jsonString);
            }
        }
    }
}
