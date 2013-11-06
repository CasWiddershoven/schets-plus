using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            foreach(Layer layer in layers)
                layer.Draw(gr);
        }

        /// <summary>Clear the canvas</summary>
        public void Schoon()
        {
            layers.Clear();
        }

        /// <summary>Rotate the canvas 90 degrees counter-clockwise around the center of the canvas</summary>
        /// <param name="xCenter">The horizontal center of the canvas</param>
        /// <param name="yCenter">The vertical center of the canvas</param>
        public void Roteer(double xCenter, double yCenter)
        {
            foreach (Layer layer in layers)
                layer.Rotate(xCenter, yCenter);
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

        /// <summary>Saves the image to the given bitmap file</summary>
        /// <param name="filename">The file to save to</param>
        /// <param name="width">The width of the bitmap</param>
        /// <param name="height">The height of the bitmap</param>
        /// <param name="format">The file format</param>
        public void saveBitmap(string filename, int width, int height, ImageFormat format)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.White);
            this.Teken(g);
            try
            {
                image.Save(filename, format);
            }
            catch (UnauthorizedAccessException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor schrijven."); }
            catch (System.Security.SecurityException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor schrijven."); }
            catch (System.IO.DirectoryNotFoundException)
            { throw new Exception("Kon het bestand niet vinden."); }
            catch (System.IO.IOException)
            { throw new Exception("Kon het bestand niet openen voor schrijven."); }
            catch (Exception)
            { throw new Exception("Een onverwachte fout is opgetreden!"); }
        }

        /// <summary>Loads an image from a bitmap file</summary>
        public void LoadBitmap(String filename)
        {
            layers.Clear();
            Bitmap image = null;
            try
            {
                image = (Bitmap)Bitmap.FromFile(filename);
            }
            catch (UnauthorizedAccessException)
            { throw new Exception("U heeft niet de juiste rechten om het bestand te openen voor lezen."); }
            catch (System.IO.DirectoryNotFoundException)
            { throw new Exception("Kon het bestand niet vinden."); }
            finally
            {
                LayerBitmap layer = new LayerBitmap(new Point(0, 0), image);
                layers.Add(layer);
            }
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
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "schets")
                            break;
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name.StartsWith("layer"))
                        {
                            Layer layer = null;
                            switch (reader.Name)
                            {
                                case LayerText.XML_NAME:
                                    layer = new LayerText(new Point(0, 0), Color.Black, "");
                                    break;

                                case LayerLine.XML_NAME:
                                    layer = new LayerLine(new Point(0, 0), new Point(1, 1), 3.0f, Color.Black);
                                    break;

                                case LayerRectFilled.XML_NAME:
                                    layer = new LayerRectFilled(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerRectOpen.XML_NAME:
                                    layer = new LayerRectOpen(new Point(0, 0), new Point(1, 1), 3.0f, Color.Black);
                                    break;

                                case LayerEllipseFilled.XML_NAME:
                                    layer = new LayerEllipseFilled(new Point(0, 0), new Point(1, 1), Color.Black);
                                    break;

                                case LayerEllipseOpen.XML_NAME:
                                    layer = new LayerEllipseOpen(new Point(0, 0), new Point(1, 1), 3.0f, Color.Black);
                                    break;

                                case LayerPath.XML_NAME:
                                    layer = new LayerPath(new Point(0, 0), 3.0f, Color.Black);
                                    break;

                                case LayerBitmap.XML_NAME:
                                    layer = new LayerBitmap(new Point(0, 0), new Bitmap(1,1));
                                    break;
                            }
                            if (layer != null)
                            {
                                layer.ReadFromXml(reader);
                                layers.Add(layer);
                            }
                        }
                        else throw new Exception(reader.Name);
                    }
                }
                catch(XmlException e)
                { throw new Exception(e.Message); }
                catch(Exception e)
                { throw new Exception("Er is een onverwachte fout opgetreden! Foutmelding:" + e.Message); }
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
