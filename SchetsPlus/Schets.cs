using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace SchetsEditor
{
    class Schets
    {
        /// <summary>The layers that form the image</summary>
        private List<Layer> layers = new List<Layer>();

        /// <summary>Property to get the layers</summary>
        public List<Layer> Layers { get { return layers; } }

        /// <summary>Draw the image</summary>
        /// <param name="gr">The graphics object that is to be used to draw the image</param>
        public void Teken(Graphics gr)
        {
            foreach(Layer layer in layers)
                layer.Draw(gr);
        }

        /// <summary>Clear the canvas</summary>
        public void Schoon()
        {
            layers.Clear();
        }

        /// <summary>Rotate the canvas</summary>
        public void Roteer()
        {
            throw new NotImplementedException(); // Implement me
        }

        /// <summary>Saves the image to the given file</summary>
        /// <param name="filename">The file to save to</param>
        public void SaveToFile(String filename)
        {
            XmlTextWriter writer = null;
            try
            {
                // Open the file
                writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);

                // Start the document and write the root element
                writer.WriteStartDocument();
                writer.WriteStartElement("schets");
                writer.WriteStartAttribute("version");
                writer.WriteValue(1);
                writer.WriteEndAttribute();

                // Write the layers
                foreach(Layer layer in layers)
                    layer.WriteToXml(writer);

                // Close the root element and save the document
                writer.WriteEndElement();
                writer.Flush();
            }
            catch(UnauthorizedAccessException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor schrijven."); }
            catch(System.Security.SecurityException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor schrijven."); }
            catch(System.IO.DirectoryNotFoundException)
            { throw new Exception("Kon het bestand niet vinden."); }
            catch(System.IO.IOException)
            { throw new Exception("Kon het bestand niet openen voor schrijven."); }
            catch(Exception)
            { throw new Exception("Een onverwachte fout is opgetreden!"); }
            finally
            { writer.Close(); }
        }
        
        /// <summary>Loads the image from the given file</summary>
        /// <param name="filename">The file to load from</param>
        public void LoadFromFile(String filename)
        {
            XmlTextReader reader = null;
            try
            {
                // Open the file
                reader = new XmlTextReader(filename);

                // Read to the root and determine the file version
                int version = 0;
                while(reader.Read())
                {
                    if(reader.NodeType == XmlNodeType.Element && reader.Name == "schets")
                    {
                        // Determine the version
                        while(reader.MoveToNextAttribute())
                        {
                            if(reader.Name == "version")
                            {
                                try
                                {
                                    version = reader.ReadContentAsInt();
                                    break;
                                }
                                catch(Exception)
                                { throw new Exception("De versie lijkt in een verkeerd formaat te staan."); }
                            }
                        }

                        break;
                    }
                }

                if(version != 1)
                    throw new Exception("Foutieve bestandsversie: " + version + ".");

                // Read the layers
                try
                {
                    layers.Clear();
                    while(reader.Read())
                    {
                        if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "schets")
                            break;
                        else if(reader.NodeType == XmlNodeType.Element && reader.Name.Substring(0, 5) == "layer")
                        {
                            Layer layer = null;
                            switch(reader.Name)
                            {
                                case LayerText.XML_NAME:
                                    layer = new LayerText(new Point(0, 0), Color.Black, "");
                                    break;

                                case LayerLine.XML_NAME:
                                    layer = new LayerLine(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerRectFilled.XML_NAME:
                                    layer = new LayerRectFilled(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerRectOpen.XML_NAME:
                                    layer = new LayerRectOpen(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerEllipseFilled.XML_NAME:
                                    layer = new LayerEllipseFilled(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerEllipseOpen.XML_NAME:
                                    layer = new LayerEllipseOpen(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerPath.XML_NAME:
                                    layer = new LayerPath(new Point(0, 0), Color.Black);
                                    break;
                            }
                            if(layer != null)
                            {
                                layer.ReadFromXml(reader);
                                layers.Add(layer);
                            }
                        }
                    }
                }
                catch(XmlException e)
                { throw new Exception(e.Message); }
                catch(Exception)
                { throw new Exception("Er is een onverwachte fout opgetreden!"); }
            }
            catch(UnauthorizedAccessException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor lezen."); }
            catch(System.IO.DirectoryNotFoundException)
            { throw new Exception("Kon het bestand niet vinden."); }
            finally
            {
                // Close the file
                reader.Close();
            }
        }
    }
}
