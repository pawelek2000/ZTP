using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM.Model
{
    public class MPlayer
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);
        public  bool play { get; set; }
        private static int Volume { get; set; }
        public static double songLength { get; set; }
        private static Stopwatch stopwatch { get; set; }
        public  MPlayer() {
            play = false;
            Volume = 500;
            songLength = 0;
            Stopwatch stopwatch = new Stopwatch();
        }
        public void Open(string file)
        {
            string command = "open \"" + file + "\" type MPEGVideo alias MyMp3";
            mciSendString(command, null, 0, 0);
            SongLength();
            stopwatch = new Stopwatch();
        }

        public void Play()
        {
            play = true;
            string command = "play MyMp3 from "+stopwatch.ElapsedMilliseconds;
            mciSendString(command, null, 0, 0);

            //SetVolume(0);
            stopwatch.Start(); 
        }

        public void Stop()
        {
            
            string command = "stop MyMp3";
            mciSendString(command, null, 0, 0);

            command = "close MyMp3";
            mciSendString(command, null, 0, 0);
            stopwatch.Stop();
        }
        public void Pause()
        {
            play = false;
            string command = "pause MyMp3";
            mciSendString(command, null, 0, 0);
            stopwatch.Stop();
        }
        public void SetVolume(int volume)
        {
            if(Volume+volume<=1000 && Volume + volume>=0)
            {
                Volume = volume;
            }
            string command = "setaudio MyMp3 volume to " + Volume;
            mciSendString(command, null, 0, 0);

        }
        public void SongLength()
        {
            StringBuilder sb = new StringBuilder(128);
            mciSendString("status MyMp3 length", sb, 128, 0);
            var songlength = (Convert.ToUInt64(sb.ToString()));
            songLength =  songlength;
        }
        public string CurrentSongTime() {
           TimeSpan t = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
            return answer;

        }
        public string SongTime()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(songLength);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
            return answer;

        }
        public double CurrentSongTimePercent()
        {
            var Percent = stopwatch.ElapsedMilliseconds/songLength;
            return Percent;
        }
        public bool SongTimeEnd() {
            if (songLength <= stopwatch.ElapsedMilliseconds) {
                return true;
            }
            else
                return false;
        }
    }
}
