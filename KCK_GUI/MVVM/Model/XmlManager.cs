using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace KCK_GUI.MVVM.Model
{
    class XmlManager
    {
        private string Path { get; set; }
        XmlDocument xmlDocument { get; set; }
        public XmlManager(string path)
        {
            Path = path;
        }

        public string getXmlFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void writeXml(XDocument xDocument, string savePath)
        {
            xDocument.Save(savePath);
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
