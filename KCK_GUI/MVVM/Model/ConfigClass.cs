using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM.Model
{
     public static class ConfigClass
    {
        public static List<MusicFile> musicFiles { get; set; }
        public static MusicFile currentSong { get; set; }
        public static string playlistPath { get; set; }
        public static bool IsCurrentSongChanged { get; set; }
        public static MPlayer Player { get; set; }

    }
}
