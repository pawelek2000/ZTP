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
using Microsoft.Win32;

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

        MusicPlayer musicPlayer { get; set; }
        MusicFilesManager musicFilesManager { get; set; }

        JsonManager FavPlaylistManager { get; set; }
        public SearchViewModel SearchVM{ get; set; }
        public PlaylistViewModel PlaylistVM { get; set; }
        public List<Song> favSongList { get; set; }
        public List<Song> CurrentSongList { get; set; }

        private BackgroundWorker _bgWorker { get; set; }


        public MainViewModel()
        {
            SearchVM = new SearchViewModel();
            PlaylistVM = new PlaylistViewModel();
            CurrentView = SearchVM;
            PlayStopImage = new Image();
            FavImage = new Image();

            musicPlayer = MusicPlayer.GetInstance();
            musicFilesManager = MusicFilesManager.GetInstance();

            FavPlaylistManager = new JsonManager("Data/fav.json");
            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = musicFilesManager.getAllSongsList();

            musicFilesManager.LoadAllMusicFiles();
            CurrentSongList = musicFilesManager.getAllSongsList();
            musicPlayer.setCurrentSong(CurrentSongList[0]);
            musicPlayer.Open();

            //ConfigClass.IsCurrentSongChanged = false;
            
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
                     if (musicPlayer.IsPlaying)
                     {

                         Thread.Sleep(100);
                         UpdateSongInfo();

                     }

                     if (musicPlayer.IsSongTimeEnd()) 
                     {
                         musicPlayer.Stop();
                     }
                   
                 }
             };
            _bgWorker.RunWorkerAsync();

            UpdateFavButton();

            SearchViewCommand = new RelayCommand(o =>
            {
                musicPlayer.SwitchIsPlaylsit(false);
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
                if (musicPlayer.IsPlaying)
                {
                    musicPlayer.Pause();
                    bi.UriSource = new Uri(PLAY_IMAGE, UriKind.Relative);
                }
                else
                {
                    musicPlayer.Play();
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
                musicPlayer.SetVolume(_volumeSlider);
                OnPropertyChanged();
            }
        }
        private int _volumeSlider;

     

        public void UpdateFavButton() 
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            var song = musicPlayer.getCurrentSong();
            if (favSongList.Find(p => p.Path == song.Path) != null)
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
            var song = musicPlayer.getCurrentSong();
            Title = song.Title;
            SongTime = musicPlayer.getSongLength();
            CurrentTime = musicPlayer.getCurrentSongTime();
            MainProgresBar = musicPlayer.getCurrentSongTimePercent();
        }

        public void PlayNext() 
        {
            ValidatePlaylist();

            if (CurrentSongList.Count > CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) + 1)
                musicPlayer.setCurrentSong(CurrentSongList[(CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) + 1)]);
            else
                musicPlayer.setCurrentSong(CurrentSongList[0]);

            PlayNew();

        }
        public void PlayPrev() 
        {
            ValidatePlaylist();

            if (0 <= CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) - 1)
                musicPlayer.setCurrentSong(CurrentSongList[(CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path))-1)]);
            else
                musicPlayer.setCurrentSong(CurrentSongList[CurrentSongList.Count -1]);

            PlayNew();
        }
        public void ValidatePlaylist()
        {
            if (musicPlayer.IsPlaylist)
                CurrentSongList = musicFilesManager.getCurrentPlaylist();
            if (CurrentSongList.Count < 2)
            {
                CurrentSongList = musicFilesManager.getAllSongsList();
            }
        }
        public void PlayNew() 
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                musicPlayer.Stop();
                musicPlayer.Open();
                UpdateFavButton();
                UpdateSongInfo();
                musicPlayer.SetVolume(VolumeSlider);
                musicPlayer.Play();
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(PAUSE_IMAGE, UriKind.Relative);
                bi.EndInit();
                PlayStopImage.Stretch = Stretch.Fill;
                PlayStopImage.Source = bi;
                //ConfigClass.IsCurrentSongChanged = false;
            }
            //else
            //{
            //    Application.Current.Dispatcher.BeginInvoke(
            //         DispatcherPriority.Background,
            //        new Action(() => {
            //            musicPlayer.Stop();
            //            musicPlayer.Open();
            //            UpdateFavButton();
            //            UpdateSongInfo();
            //            musicPlayer.SetVolume(VolumeSlider);
            //            musicPlayer.Play();
            //            BitmapImage bi = new BitmapImage();
            //            bi.BeginInit();
            //            bi.UriSource = new Uri(PAUSE_IMAGE, UriKind.Relative);
            //            bi.EndInit();
            //            PlayStopImage.Stretch = Stretch.Fill;
            //            PlayStopImage.Source = bi;
            //            //ConfigClass.IsCurrentSongChanged = false;
            //        }));
            //}

        }
        public void SerceBolesne() 
        {
            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            var song = musicPlayer.getCurrentSong();
            if (favSongList.Find(p => p.Path == song.Path) != null)
            {

                musicFilesManager.DeleteMusicFromPlaylist(song, FavPlaylistManager);
            }
            else
            {

                musicFilesManager.AddMusicToPlaylist(song, FavPlaylistManager);
            }
            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            UpdateFavButton();
        }
      
    }
  
}
