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
        public List<Song>  TempList { get; set; }
        MusicPlayer musicPlayer { get; set; }
        MusicFilesManager musicFilesManager { get; set; }


        public SearchViewModel()
        {
            
            musicPlayer = MusicPlayer.GetInstance();
            musicFilesManager = MusicFilesManager.GetInstance();
            TestText = new ObservableCollection<string>();
            Visibilities = new ObservableCollection<Visibility>();

            for (int i = 0; i < 10; i++) 
            {
                TestText.Add("");
                Visibilities.Add(Visibility.Hidden);
            }
            PlayFormSearchCommand = new RelayCommand(o =>
            {
                var button = (o as RadioButton);
                PlayFromSearch(button.Content.ToString());

            });
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                

                TempList = musicFilesManager.getAllSongsList().Where(p => p.Title.ToLower().Contains(_searchText.ToLower())).ToList();
                for (int i = 0; i < 10; i++)
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
                OnPropertyChanged();
            }
        }
        

        private string _searchText;

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

        public void PlayFromSearch(string Title)
        {
            musicFilesManager.LoadAllMusicFiles();
            musicPlayer.setCurrentSong(musicFilesManager.getAllSongsList().Find(p => p.Title == Title));
            musicPlayer.Stop();
            musicPlayer.Open();
            musicPlayer.Play();

            
        }



    }
}
