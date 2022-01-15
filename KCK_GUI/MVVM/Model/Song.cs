using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM.Model
{
    public class Song
    {
        public string Title { get; set; }
        public string Path { get; set; }
        public string Category { get; set; }
        public string Author {get; set;}
        public long Length { get; set; }
        public string Time { get; set; }
        public int Year { get; set; }
    }

    public class SongDataReader
    {
        public Song ReadData(string FileName)
        {

            string[] words = FileName.Split('_');
           
            Song song = new Song
            {
                Path = FileName,
                Title = words[0],
                Author = words[1],
                Category = words[2],
                Length = Int32.Parse(words[3]),
                Year = Int32.Parse(words[4])
            };
            TimeSpan t = TimeSpan.FromMilliseconds(song.Length);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
            song.Time = answer;

            return song;
        }


    }
}
