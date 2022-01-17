using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KCK_GUI.MVVM.Model
{
    class FormatAdapter : JsonManager
    {
        private  XmlManager xmlManager { get; set; }

        public FormatAdapter()
        {
            xmlManager = new XmlManager(Path);
        }
        public string getJsonFile()
        {
            
            string xmlTekst = xmlManager.getXmlFile();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlTekst);
            string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);

            return json;
        }

        public void writeJson(string JsonFile)
        {
            XmlDocument doc = JsonConvert.DeserializeXmlNode(JsonFile);
            //xmlManager.writeXml(doc);
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
