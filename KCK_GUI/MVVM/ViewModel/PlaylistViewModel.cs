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
        public List<MusicFile> TempList { get; set; }
        MusicPlayer Player { get; set; }
        public PlaylistViewModel()
        {
            Player = MusicPlayer.GetInstance();
            PlaylistMusicFiles = new ObservableCollection<MusicFile>();
            //TODO zmaienic na COnfigclas.pathPlaylist to samo dać jak się przechodzi pomiędzy guzik playlist wyszukiwarka
            UpdatePlaylist();
            TestText = new ObservableCollection<string>();
            Visibilities = new ObservableCollection<Visibility>();

            for (int i = 0; i < 4; i++)
            {
                TestText.Add("");
                Visibilities.Add(Visibility.Hidden);
            }

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
                var button = (o as RadioButton);
                AddToPlaylist(MusicFile.GetMusicFiles().Find(p => p.Title == button.Content.ToString()));
            });


        }
        public void UpdatePlaylist()
        {
            PlaylistMusicFiles.Clear();
            if (ConfigClass.playlistPath == null || ConfigClass.playlistPath == "")
                MusicFile.GetPlaylist("Data/fav.json").ForEach(PlaylistMusicFiles.Add);
            else
                MusicFile.GetPlaylist(ConfigClass.playlistPath).ForEach(PlaylistMusicFiles.Add);
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

        public ObservableCollection<MusicFile> PlaylistMusicFiles
        {
            get { return _playlistMusicFiles; }
            set
            {
                _playlistMusicFiles = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<MusicFile> _playlistMusicFiles;

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



        public MusicFile SelectedMusicFile
        {
            get { return _selectedMusicFile; }
            set
            {
                _selectedMusicFile = value;
                OnPropertyChanged();
            }
        }
        private MusicFile _selectedMusicFile;

        public void PlayFromPlaylist(MusicFile file)
        {
            ConfigClass.currentSong = file;
            Player.Stop();
            Player.Open();
            Player.Play();
        }
        public void DeleteFormPlaylist(MusicFile file) 
        {
            //TODO TO SAMO CO NA GÓRA
            MusicFile.DeleteMusic(file, ConfigClass.playlistPath);
            PlaylistMusicFiles.Remove(file);

        }
        public void AddToPlaylist(MusicFile file)
        {
            //TODO TO SAMO CO NA GÓRA
            UpdateSearchresults();
            MusicFile.AddMusic(file, ConfigClass.playlistPath);
            if(PlaylistMusicFiles.ToList().Find(p=>p.MusicPath==file.MusicPath)==null)
                PlaylistMusicFiles.Add(file);
            UpdateSearchresults();

        }
        public void UpdateSearchresults() 
        {
            TempList = ConfigClass.musicFiles.Where(p => p.Title.ToLower().Contains(_searchText.ToLower())).ToList();
            foreach (var item in PlaylistMusicFiles)
            {
                TempList.Remove(TempList.Find(p => p.MusicPath == item.MusicPath));
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
    }
}
