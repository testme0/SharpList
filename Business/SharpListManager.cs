using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace Business
{
    public class SharpListManager
    {
        private const string _path = "C:\\SharpList\\data.xml";
        private List<SharpList> _sharpLists;
        private XmlDocument _data;

        public SharpListManager()
        {
            FileStream fileStream = null;
            _data = new XmlDocument();

            try
            {
                fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _data.Load(fileStream);
            }
            catch (Exception e)
            {
                System.IO.Directory.CreateDirectory("C:\\SharpList");
            }
            finally
            {
                _sharpLists = new List<SharpList>();
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                    XmlNodeList xmlSharpLists = _data.GetElementsByTagName("SharpList");
                    for (int i = 0; i < xmlSharpLists.Count; i++)
                    {
                        XmlNode xmlSharpList = xmlSharpLists[i];

                        SharpList sharpList = new SharpList(xmlSharpList.Attributes[0].Value);

                        IFormatProvider culture = new CultureInfo("fr-FR", true);
                        sharpList.Date = DateTime.Parse(xmlSharpList.Attributes[1].Value,
                                           culture,
                                           DateTimeStyles.NoCurrentDateDefault);

                        XmlNodeList xmlItems = xmlSharpList.ChildNodes;

                        for (int j = 0; j < xmlItems.Count; j++)
                        {
                            XmlNode xmlItem = xmlItems[j];

                            SharpItem item = new SharpItem(xmlItem.Attributes[0].Value);
                            item.Checked = Boolean.Parse(xmlItem.InnerText);

                            sharpList.Items.Add(item);
                        }
                        _sharpLists.Add(sharpList);
                    }
                }
            }
        }

        public void CreateSharpList()
        {
            SharpList newSharpList = new SharpList("New");
            _sharpLists.Add(newSharpList);
        }

        public void DeleteSharpList(SharpList toDeleteSharpLst)
        {
            _sharpLists.Remove(toDeleteSharpLst);
        }

        public void Persist()
        {
            _data = new XmlDocument();
            XmlDeclaration xmlDeclaration = _data.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement rootNode = _data.CreateElement("Lists");
            _data.InsertBefore(xmlDeclaration, _data.DocumentElement);

            for (int i = 0; i < _sharpLists.Count; i++)
            {
                SharpList sharpList = _sharpLists[i];
                XmlElement xmlSharpList = _data.CreateElement("SharpList");

                XmlAttribute xmlSharpListName = _data.CreateAttribute("name");
                xmlSharpListName.Value = sharpList.Name;
                XmlAttribute xmlSharpListDate = _data.CreateAttribute("date");
                xmlSharpListDate.Value = String.Format("{0:dd/MM/yyyy HH:mm:ss}", sharpList.Date);

                xmlSharpList.SetAttributeNode(xmlSharpListName);
                xmlSharpList.SetAttributeNode(xmlSharpListDate);

                for (int j = 0; j < sharpList.Items.Count; j++)
                {
                    SharpItem item = sharpList.Items[j];
                    XmlElement xmlItem = _data.CreateElement("Item");

                    XmlAttribute xmlItemName = _data.CreateAttribute("name");
                    xmlItemName.Value = item.Name;

                    xmlItem.SetAttributeNode(xmlItemName);
                    xmlItem.InnerText = item.Checked.ToString();

                    xmlSharpList.AppendChild(xmlItem);
                }

                rootNode.AppendChild(xmlSharpList);
            }

            _data.AppendChild(rootNode);
            FileStream fileStream = File.Create(_path);
            _data.Save(fileStream);
            fileStream.Flush();
            fileStream.Close();
        }

        public List<SharpList> GetSharpLists()
        {
            return _sharpLists;
        }

        public void SetSharpLists(List<SharpList> sharpLists)
        {
            _sharpLists = sharpLists;
        }
    }
}
