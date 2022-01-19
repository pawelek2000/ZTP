using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM
{
    public class PlaylistManager
    {
        public string Path { get; set; }
        public string Name { get; set; }
        //public JsonManager(string path, string name)
        //{
        //    Path = path;
        //    Name = name;
        //}

        public string getJsonFile() {
            return File.ReadAllText(Path);
        }

        public void writeJson(string JsonFile)
        {
            if (File.Exists(Path))
            {
                File.WriteAllText(Path, JsonFile);
            }
            else 
            { 
            File.Create(Path).Dispose();
                
            File.WriteAllText(Path, JsonFile);
            }

        }

        public bool IsFileExisting()
        {
            if (new FileInfo(Path).Length > 8)
                return true;
            else
                return false;
        }
       
    }
}
