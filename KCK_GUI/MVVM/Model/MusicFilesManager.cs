using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private static List<Song> AllSongsList { get; set; }
        private static List<Song> CurrentPlaylist { get; set; }
        public List<Song> getAllSongsList() 
        {
            return AllSongsList;
        }

        public List<Song> getCurrentPlaylist()
        {
            return CurrentPlaylist;
        }
        public void LoadAllMusicFiles()
        {
            
            AllSongsList = new List<Song>();
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
            AllSongsList.Add(songDataReader.ReadData(path));
        }
        //Pobieranie playlisty
        public void LoadPlaylist(JsonManager jsonManager)
        {
            LoadAllMusicFiles();
            List<Song> playList = new List<Song>();
            if (jsonManager.IsFileExisting())
            {
                playList = JsonConvert.DeserializeObject<List<Song>>(jsonManager.getJsonFile());
                CurrentPlaylist = JsonConvert.DeserializeObject<List<Song>>(jsonManager.getJsonFile());

            }
            foreach (var song in playList) 
            {
                if (!AllSongsList.Contains(song)) 
                {
                    CurrentPlaylist.Remove(song);
                }
                
            }        }

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

        public void DeleteMusicFromPlaylist(Song song, JsonManager jsonManager)
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

        public string ChooseMusicFileToAdd()
        {
            var musicFilesPath = @"c:\Users\Dom\Music"; // string path = ConfigurationManager.AppSettings["MusicFilesDirectory"]
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = musicFilesPath;
            openFileDialog.Filter = "Pliki dźwiękowe (*.mp3; *.wav)| *.mp3; *.wav| Wszystkie pliki (*.*)| *.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
            }
            return filePath;
        }
        public void AddMusicFile(string title, string author, string category, double length, int year, int idNumber ,string filePath) 
        {
            var destinationMusicFilesDirectory = ConfigurationManager.AppSettings["MusicFilesDirectory"];     // Tu albo tu lol
            var finalPath = string.Empty;
            finalPath = destinationMusicFilesDirectory +"\\"+ title + "_" + author + "_" + category + "_" + length + "_" + year + "_" + idNumber + "_.mp3";

            File.Move(filePath, finalPath);
        }
        public void DeleteMusicFile(string filePath)
        {
            File.Delete(filePath);
        }

    }
}
