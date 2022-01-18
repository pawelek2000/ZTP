using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KCK_GUI.Core;
using KCK_GUI.MVVM.Model;
using System.Windows;
using System.Windows.Controls;

namespace KCK_GUI.MVVM.ViewModel
{
    class SearchViewModel : ObservableObject
    {

        public RelayCommand SearchBoxCommand { get; set; }
        public RelayCommand PlayFormSearchCommand { get; set; }
        public RelayCommand DeleteFileSearchCommand { get; set; }
        public List<Song>  TempList { get; set; }
        MusicPlayer musicPlayer { get; set; }
        MusicFilesManager musicFilesManager { get; set; }


        public SearchViewModel()
        {
            
            musicPlayer = MusicPlayer.GetInstance();
            musicFilesManager = MusicFilesManager.GetInstance();
            TestText = new ObservableCollection<Song>();

            PlayFormSearchCommand = new RelayCommand(o =>
            {
                PlayFromSearch(SelectedMusicFile);
            });

            DeleteFileSearchCommand = new RelayCommand(o=> 
            {
                musicFilesManager.DeleteMusicFile(SelectedMusicFile.Path);
                TestText.Remove(SelectedMusicFile);
            });

        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                TestText.Clear();
                TempList = musicFilesManager.getAllSongsList().Where(p => p.Title.ToLower().Contains(_searchText.ToLower())).ToList();
                for (int i = 0; i < 10; i++)
                {
                    if (TempList.Count > 0 && _searchText.Length > 0 && i < TempList.Count)
                    {
                        TestText.Add(TempList[i]);
                    }
                   
                }
                OnPropertyChanged();
            }
        }
        

        private string _searchText;

        public ObservableCollection<Song> TestText
        {
            get { return _testText; }
            set
            {
                _testText = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Song> _testText;

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
        public void PlayFromSearch(Song song)
        {
            musicFilesManager.LoadAllMusicFiles();
            musicPlayer.setCurrentSong(song);
            musicPlayer.Stop();
            musicPlayer.Open();
            musicPlayer.Play();

            
        }



    }
}
