using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLProcessingHW
{
    class XMLProcessingEntrypoint
    {
        private const string PathToXmlFile = "../../Files/catalogue.xml";

        static void Main()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(PathToXmlFile);
            XmlNode rootNode = doc.DocumentElement;

            // Problem 2: Write program that extracts all different artists which are found in the catalog.xml.
            //  For each author you should print the number of albums in the catalogue.
            //  Use the DOM parser and a hash - table. 
            PrintAlbumsForEachArtist(rootNode);

            //Problem 3: Implement the previous using XPath.
            PrintAlbumsForEachArtistUsingXPath(rootNode);

            //Problem 4: Using the DOM parser write a program to delete from catalog.xml all albums having price > 20.
            DeleteAlbumsByPrice(rootNode, 20);
            doc.Save("../../Files/catalogueNew.xml");
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
    }
}
