using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KCK_GUI.Core;
using KCK_GUI.MVVM.Model;

namespace KCK_GUI.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private string PLAY_IMAGE = @"images\icons8-circled-play-50.png";
        private string PAUSE_IMAGE = @"images\icons8-pause-button-50.png";
        private string FILLED_HEART_IMAGE = @"images\icons8-heart-50-filled.png";
        private string HEART_IMAGE = @"images\icons8-heart-50.png";
        public RelayCommand SearchViewCommand { get; set; }
        public RelayCommand PlatlistViewCommand { get; set; }
        public RelayCommand PlayStopMusicCommand { get; set; }
        public RelayCommand PlayNextCommand { get; set; }
        public RelayCommand PlayPrevCommand { get; set; }
        public RelayCommand SerceBolesneCommand { get; set; }
        

        public SearchViewModel SearchVM{ get; set; }
        public PlaylistViewModel PlaylistVM { get; set; }
        public List<MusicFile> favSongList { get; set; }

        private BackgroundWorker _bgWorker { get; set; }

        public MainViewModel()
        {
            SearchVM = new SearchViewModel();
            PlaylistVM = new PlaylistViewModel();
            CurrentView = SearchVM;
            PlayStopImage = new Image();
            FavImage = new Image();
            


            ConfigClass.Player = new MPlayer();
            ConfigClass.musicFiles = MusicFile.GetMusicFiles();
            ConfigClass.currentSong = ConfigClass.musicFiles[0];
            ConfigClass.Player.Open(ConfigClass.currentSong.MusicPath);
            ConfigClass.playlistPath = "";
            ConfigClass.IsCurrentSongChanged = false;
            favSongList = MusicFile.GetPlaylist("Data/fav.json");
            VolumeSlider = 50;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(PLAY_IMAGE, UriKind.Relative);
            bi.EndInit();
            PlayStopImage.Stretch = Stretch.Fill;
            PlayStopImage.Source = bi;

            UpdateSongInfo();

            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += (s, e) =>
             {
                 while (true)
                 {
                     if (ConfigClass.Player.play)
                     {

                         Thread.Sleep(100);
                         CurrentTime = ConfigClass.Player.CurrentSongTime();
                         MainProgresBar = ConfigClass.Player.CurrentSongTimePercent() * 100;

                     }

                     if (ConfigClass.Player.SongTimeEnd()) 
                     {
                         ConfigClass.Player.Stop();
                     }
                     if (ConfigClass.IsCurrentSongChanged) 
                     {
                         PlayNew();
                         ConfigClass.IsCurrentSongChanged = false;
                     }
                 }
             };
            _bgWorker.RunWorkerAsync();

            UpdateFavButton();

            SearchViewCommand = new RelayCommand(o =>
            {
                ConfigClass.playlistPath = "";
                CurrentView = SearchVM;

            });

            PlatlistViewCommand = new RelayCommand(o =>
            {
                var button = (o as RadioButton);
                var name = button.Name.ToString();
                switch (name)
                {
                    case  "rb1":
                        {
                            ConfigClass.playlistPath = "Data/fav.json";
                            break;
                        }
                    case "rb2":
                        {
                            ConfigClass.playlistPath = "Data/p1.json";
                            break;
                        }
                    case "rb3":
                        {
                            ConfigClass.playlistPath = "Data/p2.json";
                            break;
                        }
                    case "rb4":
                        {
                            ConfigClass.playlistPath = "Data/p3.json";
                            break;
                        }
                    case "rb5":
                        {
                            ConfigClass.playlistPath = "Data/p4.json";
                            break;
                        }
                    case "rb6":
                        {
                            ConfigClass.playlistPath = "Data/p5.json";
                            break;
                        }
                    case "rb7":
                        {
                            ConfigClass.playlistPath = "Data/p6.json";
                            break;
                        }
                    case "rb8":
                        {
                            ConfigClass.playlistPath = "Data/p7.json";
                            break;
                        }
                    case "rb9":
                        {
                            ConfigClass.playlistPath = "Data/p8.json";
                            break;
                        }
                    case "rb10":
                        {
                            ConfigClass.playlistPath = "Data/p9.json";
                           
                            break;
                        }



                }
                
                CurrentView = PlaylistVM;
                PlaylistVM.UpdatePlaylist();

            });

            PlayStopMusicCommand = new RelayCommand(o =>
            {
                bi = new BitmapImage();
                bi.BeginInit();
                if (ConfigClass.Player.play)
                {
                    ConfigClass.Player.Pause();
                    bi.UriSource = new Uri(PLAY_IMAGE, UriKind.Relative);
                }
                else
                {
                    ConfigClass.Player.Play();
                    bi.UriSource = new Uri(PAUSE_IMAGE, UriKind.Relative);
                }
                bi.EndInit();
                PlayStopImage.Stretch = Stretch.Fill;
                PlayStopImage.Source = bi;

            });
            PlayNextCommand = new RelayCommand(o =>
            {
                PlayNext();

            });
            PlayPrevCommand = new RelayCommand(o =>
            {
                PlayPrev();

            });
            SerceBolesneCommand = new RelayCommand(o =>
            {
                SerceBolesne();

            });
          

        }

        public object CurrentView
        {
            get { return _currentView; }
            set 
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private object _currentView;

        public Image PlayStopImage
        {
            get { return _playStopImage; }
            set
            {
                _playStopImage = value;
                OnPropertyChanged();
            }
        }
        private Image _playStopImage;

        public Image FavImage
        {
            get { return _favImage; }
            set
            {
                _favImage = value;
                OnPropertyChanged();
            }
        }
        private Image _favImage;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _title;

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }
        private string _currentTime;
        public string SongTime
        {
            get { return _songTime; }
            set
            {
                _songTime = value;
                OnPropertyChanged();
            }
        }
        private string _songTime;

        public double MainProgresBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                OnPropertyChanged();
            }
        }
        private double _progressBar;
        public int VolumeSlider
        {
            get { return _volumeSlider; }
            set
            {
                _volumeSlider = value;
                ConfigClass.Player.SetVolume(_volumeSlider);
                OnPropertyChanged();
            }
        }
        private int _volumeSlider;

     

        public void UpdateFavButton() 
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            favSongList = MusicFile.GetPlaylist("Data/fav.json");
            if (favSongList.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath) != null)
            {
                bi.UriSource = new Uri(FILLED_HEART_IMAGE, UriKind.Relative);
            }
            else 
            {
                bi.UriSource = new Uri(HEART_IMAGE, UriKind.Relative); ;
            }
            bi.EndInit();
            FavImage.Stretch = Stretch.Fill;
            FavImage.Source = bi;
            PlaylistVM.UpdatePlaylist();
        }
        public void UpdateSongInfo() 
        {
            Title = ConfigClass.currentSong.Title;
            SongTime = ConfigClass.Player.SongTime();
            CurrentTime = ConfigClass.Player.CurrentSongTime();
            MainProgresBar = ConfigClass.Player.CurrentSongTimePercent();
        }

        public void PlayNext() 
        {
            ValidatePlaylist();

            if (ConfigClass.musicFiles.Count > ConfigClass.musicFiles.IndexOf(ConfigClass.musicFiles.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath)) + 1)
                ConfigClass.currentSong = ConfigClass.musicFiles[(ConfigClass.musicFiles.IndexOf(ConfigClass.musicFiles.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath)) + 1)];
            else
                ConfigClass.currentSong = ConfigClass.musicFiles[0];

            PlayNew();

        }
        public void PlayPrev() 
        {
            ValidatePlaylist();

            if (0 <= ConfigClass.musicFiles.IndexOf(ConfigClass.musicFiles.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath)) - 1)
                ConfigClass.currentSong = ConfigClass.musicFiles[(ConfigClass.musicFiles.IndexOf(ConfigClass.musicFiles.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath))-1)];
            else
                ConfigClass.currentSong = ConfigClass.musicFiles[ConfigClass.musicFiles.Count -1];

            PlayNew();
        }
        public void ValidatePlaylist()
        {
            if (ConfigClass.playlistPath.Length != 0)
                ConfigClass.musicFiles = MusicFile.GetPlaylist(ConfigClass.playlistPath);

            if (ConfigClass.playlistPath.Length != 0 && ConfigClass.musicFiles.Count == 0)
                ConfigClass.musicFiles = MusicFile.GetMusicFiles();

        }
        public void PlayNew() 
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                ConfigClass.Player.Stop();
                ConfigClass.Player.Open(ConfigClass.currentSong.MusicPath);
                UpdateFavButton();
                UpdateSongInfo();
                ConfigClass.Player.SetVolume(VolumeSlider * 10);
                ConfigClass.Player.Play();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(PAUSE_IMAGE, UriKind.Relative);
                bi.EndInit();
                PlayStopImage.Stretch = Stretch.Fill;
                PlayStopImage.Source = bi;
                ConfigClass.IsCurrentSongChanged = false;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                     DispatcherPriority.Background,
                    new Action(() => {
                        ConfigClass.Player.Stop();
                        ConfigClass.Player.Open(ConfigClass.currentSong.MusicPath);
                        UpdateFavButton();
                        UpdateSongInfo();
                        ConfigClass.Player.SetVolume(VolumeSlider * 10);
                        ConfigClass.Player.Play();
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.UriSource = new Uri(PAUSE_IMAGE, UriKind.Relative);
                        bi.EndInit();
                        PlayStopImage.Stretch = Stretch.Fill;
                        PlayStopImage.Source = bi;
                        ConfigClass.IsCurrentSongChanged = false;
                    }));
            }

        }
        public void SerceBolesne() 
        {
            favSongList = MusicFile.GetPlaylist("Data/fav.json");
            if (favSongList.Find(p => p.MusicPath == ConfigClass.currentSong.MusicPath) != null)
            {
                
                MusicFile.DeleteMusic(ConfigClass.currentSong, "Data/fav.json");
            }
            else
            {
                
                MusicFile.AddMusic(ConfigClass.currentSong, "Data/fav.json");
            }
            favSongList = MusicFile.GetPlaylist("Data/fav.json");
            UpdateFavButton();
        }
      
    }
  
}
