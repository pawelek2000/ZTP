using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace KCK_GUI.MVVM.Model
{
    class FormatAdapter : PlaylistManager
    {
        private  XmlManager xmlManager { get; set; }

        public FormatAdapter()
        {
            xmlManager = new XmlManager(Path);
        }
        new public string getJsonFile()
        {
            var xml = xmlManager.getXmlFile(Path);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string jsonText = JsonConvert.SerializeXmlNode(doc);
            return jsonText;
        }

         new public void writeJson(string JsonFile)
        {
            var xml = XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(Encoding.ASCII.GetBytes(JsonFile), new XmlDictionaryReaderQuotas()));
            xmlManager.writeXml(xml, Path);
        }

         new public bool IsFileExisting()
        {
            if (new FileInfo(Path).Length > 8)
                return true;
            else
                return false;
        }
        private static Dictionary<string, object> GetXmlData(XElement xml)
        {
            var attr = xml.Attributes().ToDictionary(d => d.Name.LocalName, d => (object)d.Value);
            if (xml.HasElements) attr.Add("_value", xml.Elements().Select(e => GetXmlData(e)));
            else if (!xml.IsEmpty) attr.Add("_value", xml.Value);

            return new Dictionary<string, object> { { xml.Name.LocalName, attr } };
        }
    }
}
