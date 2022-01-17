using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KCK_GUI.MVVM.Model
{
    class FormatAdapter
    {
        private string Path;
        public FormatAdapter(string path)
        {
            Path = path;
        }

        public string convertXmlToJson()
        {

            //String xmlPath = @"C:\Users\Dom\Desktop\json\p2.xml";
            XmlManager XMLManager = new XmlManager(Path);
            var xmlTekst = XMLManager.getXmlFile();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlTekst);
            string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented, true);

            return json;
        }

    }
}
