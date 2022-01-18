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

        JsonManager FavJsonManager { get; set; }
        JsonManager PlaylistJsonManager { get; set; }
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

            JsonManagerList = new ObservableCollection<JsonManager>();
            FavJsonManager = new JsonManager
            {
                Name = "Ulubione",
                Path = "Data/fav.json"
            };

            PlaylistJsonManager = new JsonManager
            {
                Name = "Playlisty",
                Path = "Data/playlists.json"
            };         

            

            
            musicFilesManager.LoadPlaylist(FavJsonManager);
            favSongList = new List<Song>();
            favSongList = musicFilesManager.getAllSongsList();

            musicFilesManager.LoadAllMusicFiles();
            CurrentSongList = musicFilesManager.getAllSongsList();
            musicPlayer.setCurrentSong(CurrentSongList[0]);
            musicPlayer.Open();

             //for (int i = 1; i < 10; i++) 
            // {
            //   JsonManagerList.Add(new JsonManager { Path= "Data/p" + i + ".json", Name = "Playlist " + i });
           // }

            //playlistList = JsonConvert.DeserializeObject<ObservableCollection<JsonManager>>(PlaylistJsonManager.getJsonFile());

            //var siurek = JsonConvert.SerializeObject(JsonManagerList);
            //PlaylistJsonManager.writeJson(siurek);

            JsonManagerList = musicFilesManager.ReadPlaylists(PlaylistJsonManager);

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
                JsonManagerList = musicFilesManager.DeletePlaylist(SelectedJsonManager, PlaylistJsonManager , JsonManagerList);              

            });

            SearchViewCommand = new RelayCommand(o =>
            {
                JsonManagerList.Clear();
                musicPlayer.SwitchIsPlaylsit(false);
                musicFilesManager.LoadAllMusicFiles();
                CurrentSongList = musicFilesManager.getAllSongsList();
                JsonManagerList = musicFilesManager.ReadPlaylists(PlaylistJsonManager);
                CurrentView = SearchVM;

            });

            AddPlaylistViewCommand = new RelayCommand(o => 
            {
                AddNewPlaylistVM = new AddNewPlaylistViewModel();

                AddNewPlaylistVM.jsonManager = PlaylistJsonManager;

                AddNewPlaylistVM.tempList = JsonManagerList;

                CurrentView = AddNewPlaylistVM;
            });

            AddViewCommand = new RelayCommand(o =>
            {
                AddFileVM = new AddFileViewModel();
                CurrentView = AddFileVM;
            });
            PlatlistViewCommand = new RelayCommand(o =>
            {
                JsonManagerList.Clear();

                SelectedJsonManager = (o as JsonManager);
                
                CurrentView = PlaylistVM;
                PlaylistVM.CurrentJsonFile = SelectedJsonManager;
                JsonManagerList = musicFilesManager.ReadPlaylists(PlaylistJsonManager);
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

        public ObservableCollection<JsonManager> JsonManagerList
        {
            get { return _jsonManagerList; }
            set
            {
                _jsonManagerList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<JsonManager> _jsonManagerList;

        public JsonManager SelectedJsonManager
        {
            get { return _selectedJsonManager; }
            set
            {
                _selectedJsonManager = value;
                OnPropertyChanged();
            }
        }
        private JsonManager _selectedJsonManager;



        public void UpdateFavButton() 
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            musicFilesManager.LoadPlaylist(FavJsonManager);
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
                
            }
       

        }
        public void SerceBolesne() 
        {
            musicFilesManager.LoadPlaylist(FavJsonManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            var song = musicPlayer.getCurrentSong();
            if (favSongList.Find(p => p.Path == song.Path) != null)
            {

                musicFilesManager.DeleteMusicFromPlaylist(song, FavJsonManager);
            }
            else
            {

                musicFilesManager.AddMusicToPlaylist(song, FavJsonManager);
            }
            musicFilesManager.LoadPlaylist(FavJsonManager);
            favSongList = musicFilesManager.getCurrentPlaylist();
            UpdateFavButton();
        }





    }
  
}
