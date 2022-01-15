using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<Song> CurrentSongList { get; set; }
        private int SongIndex { get; set; }
        public void GetMusicFiles()
        {
            SongIndex = 0;
            CurrentSongList = new List<Song>();
            string path = ConfigurationManager.AppSettings["MusicFilesDirectory"];
            ProcessDirectory(path);

        }
        public Song GetNextSong()
        {
            return CurrentSongList[SongIndex];
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
        private  void ProcessFile(string path)
        {
            SongDataReader songDataReader = new SongDataReader();
            CurrentSongList.Add(songDataReader.ReadData(Path.GetFileName(path)));
        }


    }
}
