using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public string getXmlFile()
        {
            return File.ReadAllText(Path);
        }

        public void writeXml(string XmlFile, string savePath)
        {
            xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(XmlFile);
            xmlDocument.Save(savePath);
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
