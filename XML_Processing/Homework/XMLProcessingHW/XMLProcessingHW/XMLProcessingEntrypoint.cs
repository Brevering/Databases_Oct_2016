using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace XMLProcessingHW
{
    class XMLProcessingEntrypoint
    {
        private const string PathToXmlFile = "../../../Files/catalogue.xml";
        private const string PathToTxtFile = "../../../Files/phonebook.txt";

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
            doc.Save("../../../Files/catalogueNew.xml"); //bonus: save the new xml ;)

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
            Console.WriteLine("File phonebook.xml is created in Files folder from file phonebook.txt");
            Console.WriteLine(new string('-', 50));

            // Problem 8: Write a program, which (using XmlReader and XmlWriter) reads the file catalog.xml and creates the file 
            // album.xml, in which stores in appropriate way the names of all albums and their authors.
            CreateXmlAlbumsFileFromCatalogue(PathToXmlFile);
            Console.WriteLine("File albums.xml is created in Files folder from file catalogue.xml");
            Console.WriteLine(new string('-', 50));

            // Problem 9: Write a program to traverse given directory and write to a XML file its contents together with all subdirectories and files.
            // Use tags < file > and < dir > with appropriate attributes.
            // For the generation of the XML document use the class XmlWriter.
            using (var writer = new XmlTextWriter("../../../Files/traverseWithXmlWriter.xml", Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("Root");
                TraverseDirectoryWithXmlWriter("../../", writer); //this reads two levels up. You can substitute with any valid path
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Close();
                writer.Dispose();
            }
            Console.WriteLine("File traverseWithXmlWriter.xml is created in Files folder");
            Console.WriteLine(new string('-', 50));

            // Problem 10: Rewrite the last exercises using XDocument, XElement and XAttribute.
            var xDocument = new XDocument();
            xDocument.Add(TraverseDirectoryWithXDocument("../../"));
            xDocument.Save("../../../Files/traverseWithXDocument.xml");

            Console.WriteLine("File traverseWithXDocument.xml is created in Files folder");
            Console.WriteLine(new string('-', 50));

            // Problem 11: Write a program, which extract from the file catalog.xml the prices for all albums, published 5 years ago or earlier.
            // Use XPath query.
            XmlDocument doc2 = new XmlDocument();
            doc.Load("../../../Files/catalogue.xml");
            XmlNode root = doc.DocumentElement;
            ListPricesOfAlbumsOlderThanWithXPath(root, 2011);
            ListPricesOfAlbumsOlderThanWithXPath(root, 1983); //to demonstrate it does work, for all albums are older than five years...

            // Problem 12: Rewrite the previous using LINQ query.
            ListPricesOfAlbumsOlderThanWithLINQ(PathToXmlFile, 2011);
            ListPricesOfAlbumsOlderThanWithLINQ(PathToXmlFile, 1980);

            // Problem 13: Create an XSL stylesheet, which transforms the file catalog.xml into HTML document,
            // formatted for viewing in a standard Web-browser.
            // Problem 14: Write a C# program to apply the XSLT stylesheet transformation on the file catalog.xml
            // using the class XslTransform.
            XslCompiledTransform catalogueXslt = new XslCompiledTransform();
            catalogueXslt.Load("../../../Files/catalogue.xslt");
            catalogueXslt.Transform(PathToXmlFile, "../../../Files/catalogue.html");

            // Problem 15 - with a star
            // TODO - see later if time

            // Problem 16:
            // Using Visual Studio generate an XSD schema for the file catalog.xml.
            // Write a C# program that takes an XML file and an XSD file (schema) and validates the XML file against the schema.
            // Test it with valid XML catalogs and invalid XML catalogs.
            ValidateXmlAgainstSchema(PathToXmlFile, "../../../Files/catalogue.xsd"); // validating a valid file
            ValidateXmlAgainstSchema("../../../Files/albums.xml", "../../../Files/catalogue.xsd"); // validating an invalid file
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

            var writer = new XmlTextWriter("../../../Files/phonebook.xml", Encoding.UTF8);
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

        private static void CreateXmlAlbumsFileFromCatalogue(string pathToOriginFile)
        {
            var writer = new XmlTextWriter("../../../Files/albums.xml", Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("albums");

            using (var reader = XmlReader.Create(pathToOriginFile))
            {
                while (reader.Read())
                {
                    switch (reader.Name)
                    {
                        case "album":
                            if (reader.IsStartElement())
                            {
                                writer.WriteStartElement("album");
                            }
                            break;
                        case "name":
                            writer.WriteElementString("name", reader.ReadElementContentAsString());
                            break;
                        case "artist":
                            writer.WriteElementString("artist", reader.ReadElementContentAsString());
                            writer.WriteEndElement();
                            break;
                    }
                }
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            writer.Dispose();
        }

        private static void TraverseDirectoryWithXmlWriter(string rootDirectory, XmlWriter writer)
        {
            var directoryInfo = new DirectoryInfo(rootDirectory);
            var folders = directoryInfo.GetDirectories();

            foreach (var folder in folders)
            {
                writer.WriteStartElement("Directory");
                writer.WriteAttributeString("Name", folder.Name);
                TraverseDirectoryWithXmlWriter(folder.FullName, writer);
                writer.WriteEndElement();
            }

            var files = directoryInfo.GetFiles();

            foreach (var file in files)
            {
                writer.WriteStartElement("File");
                writer.WriteAttributeString("Name", file.Name);
                writer.WriteAttributeString("Size", file.Length.ToString());
                writer.WriteAttributeString("Modified", file.LastWriteTimeUtc.Date.ToShortDateString());
                writer.WriteEndElement();
            }



        }

        private static XElement TraverseDirectoryWithXDocument(string source)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(source);
            var result = new XElement(
                        "Directory",
                        new XAttribute("Name", directoryInfo.Name),
                        Directory.GetDirectories(source).Select(dir => TraverseDirectoryWithXDocument(dir)),
                        directoryInfo.GetFiles().Select(file => new XElement(
                                                                            "File",
                                                                            new XAttribute("Name", file.Name),
                                                                            new XAttribute("Size", file.Length),
                                                                            new XAttribute("Modified", file.LastWriteTimeUtc.Date.ToShortDateString()))));
            return result;
        }

        private static void ListPricesOfAlbumsOlderThanWithXPath(XmlNode root, int year)
        {
            var result = root.SelectNodes("album / price[../ year / text() < " + year + "]");
            Console.WriteLine($"The prices of albums published before {year} are:");
            foreach (var price in result)
            {
                Console.WriteLine((price as XmlElement).InnerXml.Trim());
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void ListPricesOfAlbumsOlderThanWithLINQ(string pathToFile, int year)
        {
            XDocument xmlDoc = XDocument.Load(pathToFile);

            var oldAlbumsPricesUsingLinq = from album in xmlDoc.Descendants("album")
                                           where int.Parse(album.Element("year").Value) < year
                                           select album.Descendants("price").FirstOrDefault();
            Console.WriteLine("--------------------using LINQ--------------------");
            Console.WriteLine($"The prices of albums published before {year} are:");

            foreach (var price in oldAlbumsPricesUsingLinq)
            {
                Console.WriteLine(price.Value.Trim());
            }

            Console.WriteLine(new string('-', 50));
        }

        private static void ValidateXmlAgainstSchema(string xmlSource, string schemaSource)
        {
            string xsdFile = File.ReadAllText(schemaSource);

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, XmlReader.Create(new StringReader(xsdFile)));

            XDocument documentToValidate = XDocument.Load(xmlSource);

            bool errors = false;

            documentToValidate.Validate(schemas, (o, e) =>
            {
                Console.WriteLine("{0}", e.Message);
                errors = true;
            });
            Console.WriteLine("The document {0}", errors ? "did not validate" : "validated");
            Console.WriteLine(new string('-', 50));
        }
    }
}
