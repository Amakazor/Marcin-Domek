using System;
using System.IO;
using System.Xml;

namespace Marcin_Domek_Console.Src
{
    public static class FileManager
    {
        private static readonly string DocumentsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\";

        public static string SaveXMLFile(string filename, string XMLString)
        {
            if (DocumentsPath != "" && Directory.Exists(DocumentsPath))
            {
                int i = 1;

                do
                {
                    if (!File.Exists(DocumentsPath + filename + "_" + i + ".xml")) break;
                } while (++i < int.MaxValue);

                if (i < int.MaxValue)
                {
                    string filepath = DocumentsPath + filename + "_" + i + ".xml";

                    XmlDocument savefile = new XmlDocument();
                    savefile.LoadXml(XMLString);
                    savefile.Save(filepath);
                    return filepath;

                }
                else return "";
            }
            else return "";
        }

        public static string LoadXMLFile(string filename)
        {
            if (File.Exists(filename))
            {
                XmlDocument loadfile = new XmlDocument();
                loadfile.Load(filename);
                return loadfile.DocumentElement.OuterXml;
            }
            else return "";
        }
    }
}
