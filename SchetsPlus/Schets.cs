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
            // Implement me
        }

        /// <summary>Saves the image to the given file</summary>
        /// <param name="filename">The file to save to</param>
        public void SaveToFile(String filename)
        {
            // Open the file
            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);

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
            writer.Close();
        }
        
        /// <summary>Loads the image from the given file</summary>
        /// <param name="filename">The file to load from</param>
        public void LoadFromFile(String filename)
        {
            // Open the file
            XmlTextReader reader = new XmlTextReader(filename);

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
                            version = reader.ReadContentAsInt();
                            break;
                        }
                    }

                    break;
                }
            }

            // TODO: report an error when the version is invalid

            // Read the layers
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

            // Close the file
            reader.Close();
        }
    }
}
