using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KCK_GUI.Core;
using KCK_GUI.MVVM.Model;

namespace KCK_GUI.MVVM.ViewModel
{
    class AddFileViewModel : ObservableObject
    {
        public RelayCommand FindFileCommand { get; set; }
        public RelayCommand AddFileCommand { get; set; }

        MusicFilesManager musicFilesManager { get; set; }
        MusicPlayer musicPlayer { get; set; }
        public AddFileViewModel()
        {
            AddingSong = new Song();
            musicFilesManager = MusicFilesManager.GetInstance();
            musicPlayer = MusicPlayer.GetInstance();

            FindFileCommand = new RelayCommand(o => 
            {
                FilePath = musicFilesManager.ChooseMusicFileToAdd();
            });

            AddFileCommand = new RelayCommand(o =>
            {
                if (FilePath != null && Title != null && !Title.Contains('_') && Author != null && !Author.Contains('_') && Category != null && !Category.Contains('_') && Year > 1500 && Year < 2023)
                {
                    double length = musicPlayer.getSongLengthInMilliseconds();
                    Random random = new Random();
                    int randomNumber = random.Next();
                    musicFilesManager.AddMusicFile(Title, Author, Category, length, Year, randomNumber, FilePath);
                }
                else
                    MessageBox.Show("Źle");
            });


        }

        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }
        private string _filePath;

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

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }
        private string _category;

        public string Author
        {
            get { return _author; }
            set
            {
                _author = value;
                OnPropertyChanged();
            }
        }
        private string _author;

        public int Year
        {
            get { return _year; }
            set
            {
                _year = value;
                OnPropertyChanged();
            }
        }
        private int _year;

        public Song AddingSong
        {
            get { return _addingSong; }
            set
            {
                _addingSong = value;
                OnPropertyChanged();
            }
        }
        private Song _addingSong;
    }
}
