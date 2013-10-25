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
    }
}
