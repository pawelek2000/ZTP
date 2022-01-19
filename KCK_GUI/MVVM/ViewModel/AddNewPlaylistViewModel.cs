using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCK_GUI.Core;
using KCK_GUI.MVVM.Model;

namespace KCK_GUI.MVVM.ViewModel
{
    class AddNewPlaylistViewModel : ObservableObject
    {

        public RelayCommand CreatePlaylistCommand { get; set; }
        public PlaylistManager jsonManager { get; set; }
        public MusicFilesManager musicFilesManager { get; set; }

        public ObservableCollection<PlaylistManager> tempList { get; set; }
        
        public AddNewPlaylistViewModel()
        {
            musicFilesManager = MusicFilesManager.GetInstance();

            CreatePlaylistCommand = new RelayCommand(o => {


                if (playlistName != null)
                {
                    var path = "Data/" + playlistName + ".json";
                    //File.Create(path);

                    var newJsonManager = new PlaylistManager
                    {
                        Name = playlistName,
                        Path = path
                    };
                    File.Create(path).Dispose();
                    musicFilesManager.AddPlaylist(newJsonManager, jsonManager, tempList);
                }
               
            });
        }
        
        public string playlistName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        private string _name;
    }
}
