using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KCK_GUI.Core;
using KCK_GUI.MVVM.Model;

namespace KCK_GUI.MVVM.ViewModel
{
    class PlaylistViewModel : ObservableObject
    {
        public RelayCommand PlayFormPlaylistCommand { get; set; }
        public RelayCommand DeleteFromPlaylistCommand { get; set; }
        public RelayCommand AddToPlaylistCommand { get; set; }
        public RelayCommand SortByTitleCommand { get; set; }
        public RelayCommand SortByYearCommand { get; set; }
        public RelayCommand SortByDurationCommand { get; set; }
        public List<Song> TempList { get; set; }
        MusicPlayer musicPlayer { get; set; }
        MusicFilesManager musicFilesManager { get; set; }
        public JsonManager CurrentJsonFile { get; set; }
        public bool TitleSort { get; set; }
        public bool YearSort { get; set; }
        public bool DurationSort { get; set; }
        public PlaylistViewModel()
        {
            musicPlayer = MusicPlayer.GetInstance();
            musicFilesManager = MusicFilesManager.GetInstance();
            CurrentSongList = new ObservableCollection<Song>();
            TitleSort = false;
            YearSort = false;
            DurationSort = false;
            TestText = new ObservableCollection<string>();
            Visibilities = new ObservableCollection<Visibility>();

            for (int i = 0; i < 4; i++)
            {
                TestText.Add("");
                Visibilities.Add(Visibility.Hidden);
            }
            SortByTitleCommand = new RelayCommand(o=> 
            {
                CurrentSongList = SortByTitle(CurrentSongList);
            });
            SortByYearCommand = new RelayCommand(o =>
            {
                CurrentSongList = SortByYear(CurrentSongList);
            });
            SortByDurationCommand = new RelayCommand(o =>
            {
                CurrentSongList = SortByDuration(CurrentSongList);
            });
            PlayFormPlaylistCommand = new RelayCommand(o =>
            {
                PlayFromPlaylist(SelectedMusicFile);
            });

            DeleteFromPlaylistCommand = new RelayCommand(o =>
            {
                DeleteFormPlaylist(SelectedMusicFile);
            });
            AddToPlaylistCommand = new RelayCommand(o =>
            {
                musicFilesManager.LoadAllMusicFiles();
                var button = (o as RadioButton);
                AddToPlaylist(musicFilesManager.getAllSongsList().Find(p => p.Title == button.Content.ToString()));
            });


        }
       
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                UpdateSearchresults();
                OnPropertyChanged();
            }
        }

        private string _searchText;

        public ObservableCollection<Song> CurrentSongList
        {
            get { return _currentSongList; }
            set
            {
                _currentSongList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Song> _currentSongList;

        public ObservableCollection<string> TestText
        {
            get { return _testText; }
            set
            {
                _testText = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _testText;

        public ObservableCollection<Visibility> Visibilities
        {
            get { return _visibilities; }
            set
            {
                _visibilities = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Visibility> _visibilities;



        public Song SelectedMusicFile
        {
            get { return _selectedMusicFile; }
            set
            {
                _selectedMusicFile = value;
                OnPropertyChanged();
            }
        }
        private Song _selectedMusicFile;

        public void PlayFromPlaylist(Song file)
        {
            musicPlayer.setCurrentSong(file);
            musicPlayer.Stop();
            musicPlayer.Open();
            musicPlayer.Play();
        }
        public void DeleteFormPlaylist(Song file) 
        {
            musicFilesManager.DeleteMusicFromPlaylist(file, CurrentJsonFile);
            CurrentSongList.Remove(file);

        }
        public void AddToPlaylist(Song file)
        {
            UpdateSearchresults();
            musicFilesManager.AddMusicToPlaylist(file, CurrentJsonFile);
            if(CurrentSongList.ToList().Find(p=>p.Path==file.Path)==null)
                CurrentSongList.Add(file);
            UpdateSearchresults();

        }
        public void UpdateSearchresults() 
        {
            musicFilesManager.LoadAllMusicFiles();
            TempList = musicFilesManager.getAllSongsList().Where(p => p.Title.ToLower().Contains(_searchText.ToLower())).ToList();
            foreach (var item in CurrentSongList)
            {
                TempList.Remove(TempList.Find(p => p.Path == item.Path));
            }
            for (int i = 0; i < 4; i++)
            {
                if (TempList.Count > 0 && _searchText.Length > 0 && i < TempList.Count)
                {
                    TestText[i] = TempList[i].Title;
                    Visibilities[i] = Visibility.Visible;
                }
                else
                {
                    TestText[i] = "";
                    Visibilities[i] = Visibility.Hidden;
                }
            }
        }
        public void UpdatePlaylist() 
        {
            CurrentSongList.Clear();
            if (CurrentJsonFile != null)
            {
                musicFilesManager.LoadPlaylist(CurrentJsonFile);
                if (musicFilesManager.getCurrentPlaylist()!=null)
                    musicFilesManager.getCurrentPlaylist().ForEach(CurrentSongList.Add);
            }
        }
        public ObservableCollection<Song> SortByYear(ObservableCollection<Song> Songs)
        {
            if (YearSort)
            {
                TitleSort = false;
                YearSort = false;
                DurationSort = false;
                return Songs = new ObservableCollection<Song>(Songs.Reverse());
            }
            else
            {
                TitleSort = false;
                YearSort = true;
                DurationSort = false;
                return Songs = new ObservableCollection<Song>(Songs.OrderBy(p => p.Year));
            }
        }
        public ObservableCollection<Song> SortByDuration(ObservableCollection<Song> Songs)
        {

            if (DurationSort)
            {
                TitleSort = false;
                YearSort = false;
                DurationSort = false;
                return Songs = new ObservableCollection<Song>(Songs.Reverse());
            }
            else
            {
                TitleSort = false;
                YearSort = false;
                DurationSort = true;
                return Songs = new ObservableCollection<Song>(Songs.OrderBy(p => p.Length));
            }
            
        }

        public ObservableCollection<Song> SortByTitle(ObservableCollection<Song> Songs)
        {
            if (TitleSort)
            {
                TitleSort = false;
                YearSort = false;
                DurationSort = false;
                return Songs = new ObservableCollection<Song>(Songs.Reverse());
            }
            else
            {
                TitleSort = true;
                YearSort = false;
                DurationSort = false;
                return Songs = new ObservableCollection<Song>(Songs.OrderBy(p => p.Length));
            }
        }
    }
}
