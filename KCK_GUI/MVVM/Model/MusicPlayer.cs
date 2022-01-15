using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM.Model
{
    public sealed class MusicPlayer
    {
        
        private MusicPlayer() 
        {
            IsPlaying = false;
            Volume = 50;
            SongLength = 0;
            MusicPlayerStopwatch = new Stopwatch();
        }

        private static MusicPlayer _instance;

        public static MusicPlayer GetInstance() 
        {
            if (_instance == null)
            {
                _instance = new MusicPlayer();
            }
            return _instance;
        }
        //Logika
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwndCallback);
        public bool IsPlaying { get; set; }
        private static int Volume { get; set; }
        private static double SongLength { get; set; }
        private static Stopwatch MusicPlayerStopwatch { get; set; }

        public void Open(string file)
        {
            string command = "open \"" + file + "\" type MPEGVideo alias MyMp3";
            mciSendString(command, null, 0, 0);
            UpdateSongLength();
            MusicPlayerStopwatch = new Stopwatch();
        }

        public void Play()
        {
            IsPlaying = true;
            string command = "play MyMp3 from " + MusicPlayerStopwatch.ElapsedMilliseconds;
            mciSendString(command, null, 0, 0);

            SetVolume(Volume);
            MusicPlayerStopwatch.Start();
        }

        public void Stop()
        {

            string command = "stop MyMp3";
            mciSendString(command, null, 0, 0);

            command = "close MyMp3";
            mciSendString(command, null, 0, 0);
            MusicPlayerStopwatch.Stop();
        }

        public void Pause()
        {
            IsPlaying = false;
            string command = "pause MyMp3";
            mciSendString(command, null, 0, 0);
            MusicPlayerStopwatch.Stop();
        }
        public void SetVolume(int volume)
        {
            
            Volume = volume;
            string command = "setaudio MyMp3 volume to " + Volume;
            mciSendString(command, null, 0, 0);

        }

        public string getCurrentSongTime()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(MusicPlayerStopwatch.ElapsedMilliseconds);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
            return answer;

        }

        public string getSongLength()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(SongLength);
            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds
                                    );
            return answer;

        }

        public double getCurrentSongTimePercent()
        {
            var Percent = MusicPlayerStopwatch.ElapsedMilliseconds / SongLength * 100;
            return Percent;
        }
        public bool IsSongTimeEnd()
        {
            if (SongLength <= MusicPlayerStopwatch.ElapsedMilliseconds)
            {
                return true;
            }
            else
                return false;
        }

        //Private
        private void UpdateSongLength()
        {
            StringBuilder sb = new StringBuilder(128);
            mciSendString("status MyMp3 length", sb, 128, 0);
            var songlength = (Convert.ToUInt64(sb.ToString()));
            SongLength = songlength;
        }
    }
}
