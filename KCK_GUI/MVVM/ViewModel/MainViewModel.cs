using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Newtonsoft.Json;

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
        public RelayCommand AddViewCommand { get; set; }
        public RelayCommand AddPlaylistViewCommand { get; set; }
        public RelayCommand DeletePlaylistCommand{ get; set; }

        MusicPlayer musicPlayer { get; set; }
        MusicFilesManager musicFilesManager { get; set; }

        public PlaylistManager FavPlaylistManager { get; set; }
        public PlaylistManager ReadPlaylistManager { get; set; }
        public SearchViewModel SearchVM { get; set; }
        public PlaylistViewModel PlaylistVM { get; set; }
        public AddFileViewModel AddFileVM { get; set; }

        public AddNewPlaylistViewModel AddNewPlaylistVM {get; set;}
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

            PlaylistsList = new ObservableCollection<PlaylistManager>();
            FavPlaylistManager = new PlaylistManager
            {
                Name = "Ulubione",
                Path = "Data/fav.json"
            };
            
            ReadPlaylistManager = new PlaylistManager
            {
                Name = "Playlisty",
                Path = "Data/playlists.json"
            };

           


            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = new List<Song>();
            favSongList = musicFilesManager.getAllSongsList();

            musicFilesManager.LoadAllMusicFiles();
            CurrentSongList = musicFilesManager.getAllSongsList();
            if (CurrentSongList.Count > 0)
            {
                musicPlayer.setCurrentSong(CurrentSongList[0]);
                musicPlayer.Open();
            }
            
            PlaylistsList = musicFilesManager.ReadPlaylists(ReadPlaylistManager);

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

            DeletePlaylistCommand = new RelayCommand(o => 
            {
                if(SelectedPlaylistManager.Path!= "Data/fav.json")
                PlaylistsList = musicFilesManager.DeletePlaylist(SelectedPlaylistManager, ReadPlaylistManager , PlaylistsList);              

            });

            SearchViewCommand = new RelayCommand(o =>
            {
                PlaylistsList.Clear();
                musicPlayer.SwitchIsPlaylsit(false);
                musicFilesManager.LoadAllMusicFiles();
                CurrentSongList = musicFilesManager.getAllSongsList();
                PlaylistsList = musicFilesManager.ReadPlaylists(ReadPlaylistManager);
                CurrentView = SearchVM;

            });

            AddPlaylistViewCommand = new RelayCommand(o => 
            {
                AddNewPlaylistVM = new AddNewPlaylistViewModel();

                AddNewPlaylistVM.jsonManager = ReadPlaylistManager;

                AddNewPlaylistVM.tempList = PlaylistsList;

                CurrentView = AddNewPlaylistVM;
            });

            AddViewCommand = new RelayCommand(o =>
            {
                AddFileVM = new AddFileViewModel();
                CurrentView = AddFileVM;
            });
            PlatlistViewCommand = new RelayCommand(o =>
            {
                PlaylistsList.Clear();

                SelectedPlaylistManager = (o as PlaylistManager);
                
                CurrentView = PlaylistVM;
                PlaylistVM.CurrentJsonFile = SelectedPlaylistManager;
                PlaylistsList = musicFilesManager.ReadPlaylists(ReadPlaylistManager);
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

        public ObservableCollection<PlaylistManager> PlaylistsList
        {
            get { return _jsonManagerList; }
            set
            {
                _jsonManagerList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<PlaylistManager> _jsonManagerList;

        public PlaylistManager SelectedPlaylistManager
        {
            get { return _selectedJsonManager; }
            set
            {
                _selectedJsonManager = value;
                OnPropertyChanged();
            }
        }
        private PlaylistManager _selectedJsonManager;



        public void UpdateFavButton() 
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            musicFilesManager.LoadPlaylist(FavPlaylistManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            var song = musicPlayer.getCurrentSong();
            if (favSongList !=null && favSongList.Find(p => p.Path == song.Path) != null)
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
            if (song != null)
            {
                Title = song.Title;
                SongTime = musicPlayer.getSongLength();
                CurrentTime = musicPlayer.getCurrentSongTime();
                MainProgresBar = musicPlayer.getCurrentSongTimePercent();
            }
        }

        public void PlayNext() 
        {
            if (CurrentSongList.Count > 0) { 
            ValidatePlaylist();

            if (CurrentSongList.Count > CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) + 1)
                musicPlayer.setCurrentSong(CurrentSongList[(CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) + 1)]);
            else
                musicPlayer.setCurrentSong(CurrentSongList[0]);

            PlayNew();
            }

        }
        public void PlayPrev() 
        {
            if (CurrentSongList.Count > 0)
            {
                ValidatePlaylist();

                if (0 <= CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) - 1)
                    musicPlayer.setCurrentSong(CurrentSongList[(CurrentSongList.IndexOf(CurrentSongList.Find(p => p.Path == musicPlayer.getCurrentSong().Path)) - 1)]);
                else
                    musicPlayer.setCurrentSong(CurrentSongList[CurrentSongList.Count - 1]);

                PlayNew();
            }
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
                
            }
       

        }
        public void SerceBolesne() 
        {
            if (CurrentSongList.Count > 0)
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
  
}
