using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCK_GUI.MVVM.Model
{
    class XmlManager
    {
        private string Path;
        public XmlManager(string path)
        {
            Path = path;
        }

        public string getXmlFile()
        {
            return File.ReadAllText(Path);
        }

        public void writeXml(string XmlFile)
        {
            File.WriteAllText(Path, XmlFile);
        }

        public bool isFileExisting()
        {
            if (new FileInfo(Path).Length > 8)
                return true;
            else
                return false;
        }
    }
}
