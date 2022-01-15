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
    public class MusicFile
    {
        public string MusicPath { get; set; }
        public string Title { get; set; }

        private static List<MusicFile> musicFiles { get;set;}
        public static List<MusicFile> GetMusicFiles() {
            musicFiles = new List<MusicFile>();
            string path = ConfigurationManager.AppSettings["MusicFilesDirectory"];
            ProcessDirectory(path);


            return musicFiles;
        }
        private static  void ProcessDirectory(string targetDirectory)
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
        public static void ProcessFile(string path)
        {
            musicFiles.Add(new MusicFile {MusicPath =path, Title = Path.GetFileName(path)});
        }



        public static List<MusicFile> GetPlaylist(string path)
        {
            List<MusicFile> playList = new List<MusicFile>();
            if (new FileInfo(path).Length > 8)
            {
                string jsonString = System.IO.File.ReadAllText(path);
                playList = JsonConvert.DeserializeObject<List<MusicFile>>(jsonString);
            }
            List<MusicFile> files = MusicFile.GetMusicFiles();
            foreach (var music in playList)
            {
                if (!files.Contains(music))
                    musicFiles.Remove(music);
            }
            return playList;
        }
        public static void AddMusic(MusicFile musicFile,string path)
        {
            List<MusicFile> playList = new List<MusicFile>();
            if (new FileInfo(path).Length > 8)
            {
                string jsonString = System.IO.File.ReadAllText(path);
                playList = JsonConvert.DeserializeObject<List<MusicFile>>(jsonString);
            }
            if (!playList.Contains(playList.Find(p => p.MusicPath == musicFile.MusicPath)))
            {
                playList.Add(musicFile);
                string jsonString = JsonConvert.SerializeObject(playList);
                System.IO.File.WriteAllText(path, jsonString);
            }

        }
        public static void DeleteMusic(MusicFile musicFile,string path)
        {
            List<MusicFile> playList = new List<MusicFile>();
            if (new FileInfo(path).Length > 8)
            {
                string jsonString = System.IO.File.ReadAllText(path);
                playList = JsonConvert.DeserializeObject<List<MusicFile>>(jsonString);
            }
            if (playList.Contains(playList.Find(p => p.MusicPath == musicFile.MusicPath)))
            {
                playList.Remove(playList.Find(p => p.MusicPath == musicFile.MusicPath));
                string jsonString = JsonConvert.SerializeObject(playList);
                System.IO.File.WriteAllText(path, jsonString);

            }
        }
      
    }

    
}
