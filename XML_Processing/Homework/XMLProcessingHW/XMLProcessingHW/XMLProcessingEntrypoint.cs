using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLProcessingHW
{
    class XMLProcessingEntrypoint
    {
        private const string PathToXmlFile = "../../Files/catalogue.xml";
        private const string PathToTxtFile = "../../Files/phonebook.txt";

        static void Main()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToXmlFile);
            XmlNode rootNode = doc.DocumentElement;

            // Problem 2: Write program that extracts all different artists which are found in the catalog.xml.
            //  For each author you should print the number of albums in the catalogue.
            //  Use the DOM parser and a hash - table. 
            PrintAlbumsForEachArtist(rootNode);

            // Problem 3: Implement the previous using XPath.
            PrintAlbumsForEachArtistUsingXPath(rootNode);

            // Problem 4: Using the DOM parser write a program to delete from catalog.xml all albums having price > 20.
            DeleteAlbumsByPrice(rootNode, 20);
            doc.Save("../../Files/catalogueNew.xml"); //bonus: save the new xml ;)

            // Problem 5: Write a program, which using XmlReader extracts all song titles from catalog.xml
            Console.WriteLine("All song titles in catalogue are:"
                                + Environment.NewLine
                                + string.Join(", ", ReturnSongTitles(PathToXmlFile).ToArray()));
            Console.WriteLine(new string('-', 50));

            // Problem 6: Rewrite the same using XDocument and LINQ query
            Console.WriteLine("All song titles in catalogue extracted with LINQ are:"
                                + Environment.NewLine
                                + string.Join(", ", ReturnSongTitlesWithLINQ(PathToXmlFile).ToArray()));
            Console.WriteLine(new string('-', 50));

            // Problem 7: In a text file we are given the name, address and phone number of given person (each at a single line).
            //  Write a program, which creates new XML document, which contains these data in structured XML format.
                        CreateXmlPhonebook(PathToTxtFile);

        }

        private static void PrintAlbumsForEachArtist(XmlNode root)
        {
            var artists = new Hashtable();

            foreach (XmlElement album in root.ChildNodes)
            {
                if (artists.ContainsKey(album["artist"].InnerText))
                {
                    (artists[album["artist"].InnerText] as List<string>).Add(album["name"].InnerText);
                }
                else
                {
                    artists.Add(album["artist"].InnerText, new List<string> { album["name"].InnerText });
                }
            }

            Console.WriteLine("Number of albums by artist using DOM parser");


            foreach (var key in artists.Keys)
            {
                Console.WriteLine($"{key} Number of albums: {(artists[key] as List<string>).Count}");
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void PrintAlbumsForEachArtistUsingXPath(XmlNode root)
        {
            string XPathQueryString = "album/artist";
            XmlNodeList artistsList = root.SelectNodes(XPathQueryString);
            var artistsXPath = new Hashtable();

            foreach (XmlElement artist in artistsList)
            {
                if (artistsXPath.ContainsKey(artist.InnerText))
                {
                    (artistsXPath[artist.InnerText] as List<string>).Add(artist.ParentNode["name"].InnerText);
                }
                else
                {
                    artistsXPath.Add(artist.InnerText, new List<string> { artist.ParentNode["name"].InnerText });
                }
            }

            Console.WriteLine("Number of albums by artist using XPath");

            foreach (var key in artistsXPath.Keys)
            {
                Console.WriteLine($"{key} Number of albums: {(artistsXPath[key] as List<string>).Count}");
            }

            Console.WriteLine(new string('-', 50));

        }

        private static void DeleteAlbumsByPrice(XmlNode root, double minPrice)
        {
            bool deletePrevious = false;
            int deletedCount = 0;

            foreach (XmlElement album in root.ChildNodes)
            {
                if (deletePrevious)
                {
                    root.RemoveChild(album.PreviousSibling);
                    deletePrevious = false;
                }

                if (double.Parse(album["price"].InnerText) > minPrice)
                {
                    deletePrevious = true;
                    deletedCount++;
                }
            }

            if (deletePrevious)
            {
                root.RemoveChild(root.LastChild);
            }

            Console.WriteLine($"{deletedCount} albums with price over {minPrice} deleted!");
            Console.WriteLine(new string('-', 50));
        }

        private static List<string> ReturnSongTitles(string pathToFile)
        {
            var titles = new List<string>();

            using (XmlReader reader = XmlReader.Create(pathToFile))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "title"))
                    {
                        titles.Add(reader.ReadElementString().Trim());
                    }
                }
            }

            return titles;
        }

        private static List<string> ReturnSongTitlesWithLINQ(string pathToFile)
        {
            var titlesList = new List<string>();

            XDocument xmlDoc = XDocument.Load(pathToFile);
            var titles =
                from title in xmlDoc.Descendants("title")
                select title.Value.Trim();
            foreach (var item in titles)
            {
                titlesList.Add(item);
            }

            return titlesList;
        }

        private static void CreateXmlPhonebook(string pathToPhonebook)
        {
            int counter = 0;

            var writer = new XmlTextWriter("../../Files/phonebook.xml", Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("entries");

            using (var reader = new StreamReader(pathToPhonebook))
            {
                while (!reader.EndOfStream)
                {
                    switch (counter % 3)
                    {
                        case 0:
                            writer.WriteStartElement("entry");
                            writer.WriteElementString("name", reader.ReadLine());
                            break;
                        case 1:
                            writer.WriteElementString("address", reader.ReadLine());
                            break;
                        case 2:
                            writer.WriteElementString("phone", reader.ReadLine());
                            writer.WriteEndElement();                            
                            break;
                    }

                    counter++;
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            writer.Dispose();
        }
    }
}
