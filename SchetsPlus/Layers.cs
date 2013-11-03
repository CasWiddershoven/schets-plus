using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

namespace SchetsEditor
{
    /// <summary>Represents a single layer in the drawing</summary>
    abstract class Layer
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="col">The color</param>
        public Layer(Point loc, Color col)
        {
            location = loc;
            color = col;
        }

        /// <summary>The location of the layer</summary>
        protected Point location;
        /// <summary>Property to get or set the location of the layer</summary>
        public virtual Point Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>The color of the layer</summary>
        protected Color color;
        /// <summary>Property to get or set the color of the layer</summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public abstract void Draw(Graphics g);

        /// <summary>Rotates the layer</summary>
        /// <param name="xCenter">The x center to rotate around</param>
        /// <param name="yCenter">The y center to rotate around</param>
        public abstract void Rotate(double xCenter, double yCenter);

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public const String XML_NAME = "layer";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public virtual String XmlName { get { return XML_NAME; } }

        /// <summary>Writes this layer to a XML document</summary>
        /// <param name="writer">The XML document to write to</param>
        public void WriteToXml(XmlWriter writer)
        {
            writer.WriteStartElement(XmlName);
            writeDataToXml(writer);
            writer.WriteEndElement();
        }

        /// <summary>Writes the data that compose this layer to a XML document.</summary>
        /// <param name="writer">The XML document to write to</param>
        protected virtual void writeDataToXml(XmlWriter writer)
        {
            // The location
            writer.WriteStartElement("location");
            writer.WriteStartElement("x");
            writer.WriteValue(location.X);
            writer.WriteEndElement();
            writer.WriteStartElement("y");
            writer.WriteValue(location.Y);
            writer.WriteEndElement();
            writer.WriteEndElement();

            // The color (as a 32-bit ARGB value)
            writer.WriteStartElement("color");
            writer.WriteValue(color.ToArgb());
            writer.WriteEndElement();
        }

        /// <summary>Reads the data for this layer from a XML document</summary>
        /// <param name="reader">The XML document to read from</param>
        public void ReadFromXml(XmlReader reader)
        {
            while(reader.Read())
            {
                if(reader.NodeType == XmlNodeType.Element)
                    readDataFromXml(reader);
                else if(reader.NodeType == XmlNodeType.EndElement && reader.Name == XmlName)
                    break;
            }
        }

        /// <summary>Called when the current node in the XmlReader should be parsed into data for this layer</summary>
        /// <param name="reader">The XmlReader that holds the current node</param>
        protected virtual void readDataFromXml(XmlReader reader)
        {
            // Read the location
            if(reader.Name == "location")
            {
                reader.Read();
                while(reader.NodeType != XmlNodeType.EndElement || reader.Name != "location")
                {
                    if(reader.NodeType == XmlNodeType.Element && (reader.Name == "x" || reader.Name == "y"))
                    {
                        if(reader.Name == "x")
                            location.X = reader.ReadElementContentAsInt();
                        else
                            location.Y = reader.ReadElementContentAsInt();
                    }
                }
            }
            // Read the color
            else if(reader.Name == "color")
            {
                reader.Read();
                if(reader.NodeType == XmlNodeType.Text)
                {
                    try { color = Color.FromArgb(reader.ReadContentAsInt()); }
                    catch(Exception)
                    { throw new XmlException("Verkeerd kleurformaat."); }
                }
                else
                    throw new XmlException("Een 'text node' werd verwacht.");

                if(reader.NodeType != XmlNodeType.EndElement || reader.Name != "color")
                    throw new XmlException("Onverwachte 'node', de node '</color>' werd verwacht.");
            }
        }

        /// <summary>The error we allow for in the IsClicked() method.
        /// That is, the maximum distance that the given points may be away from the actual layer.</summary>
        public const int ALLOWED_ERROR = 5;

        /// <summary>Whether the layer is clicked or not when clicking at the given position</summary>
        /// <param name="pos">The position where user clicked</param>
        /// <returns>True if the layer is clicked, otherwise false</returns>
        public abstract bool IsClicked(Point pos);
    }

    /// <summary>Represents a layer that contains some text</summary>
    class LayerText : Layer
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location (top-left corner of the string)</param>
        /// <param name="col">The color</param>
        /// <param name="str">The string</param>
        public LayerText(Point loc, Color col, String str) : base(loc, col)
        { text = str; }

        /// <summary>The text that is to be displayed</summary>
        protected String text;
        /// <summary>Property to set or get the text that is to be displayed</summary>
        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>Keeps track of the bottom right corner of the bounding rectangle</summary>
        private Point bottomRight;

        /// <summary>Whether or not the layer is currently being editted</summary>
        protected bool editting = false;
        /// <summary>Property to set or get whether the layer is currently being editted</summary>
        public bool Editting
        {
            get { return editting; }
            set { editting = value; }
        }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Matrix oldTransform = g.Transform; // Save the old transform
            g.Transform = rotationMatrix; // And rotate g for now
            Font font = new Font("Tahoma", 40);
            if(editting)
            {
                SizeF textSize = g.MeasureString(text, font);
                g.DrawRectangle(new Pen(Color.Gray, 3), location.X, location.Y, textSize.Width, textSize.Height);
                bottomRight = new Point(location.X + (int)textSize.Width, location.Y + (int)textSize.Height);
            }
            g.DrawString(text, font, new SolidBrush(color), location);
            g.Transform = oldTransform; // Reinstate the old transform
        }

        /// <summary>The angle at which the layer is drawn</summary>
        private int angle = 0;
        /// <summary>The center of the image at the time when the rotationMatrix was created</summary>
        private Point center;

        /// <summary>The matrix used for rotating the layer</summary>
        private Matrix rotationMatrix = new Matrix();

        /// <summary>Rotates the layer</summary>
        /// <param name="xCenter">The x center to rotate around</param>
        /// <param name="yCenter">The y center to rotate around</param>
        public override void Rotate(double xCenter, double yCenter)
        {
            angle += 90;
            rotationMatrix = new Matrix();
            center = new Point((int)xCenter, (int)yCenter);
            rotationMatrix.RotateAt(-angle, center);
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-text";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        /// <summary>Writes the data that compose this layer to a XML document.</summary>
        /// <param name="writer">The XML document to write to</param>
        protected override void writeDataToXml(XmlWriter writer)
        {
            // Call the base method
            base.writeDataToXml(writer);

            // The text
            writer.WriteStartElement("text");
            writer.WriteString(text);
            writer.WriteEndElement();
        }

        /// <summary>Called when the current node in the XmlReader should be parsed into data for this layer</summary>
        /// <param name="reader">The XmlReader that holds the current node</param>
        protected override void readDataFromXml(XmlReader reader)
        {
            // Read the location
            if(reader.Name == "text")
            {
                reader.Read();
                if(reader.NodeType == XmlNodeType.Text)
                    text = reader.ReadContentAsString();
                else
                    throw new XmlException("Een 'text node' werd verwacht.");

                if(reader.NodeType != XmlNodeType.EndElement || reader.Name != "text")
                    throw new XmlException("Onverwachte 'node', de node '</text>' werd verwacht.");
            }
            // Let the base class read its data
            else
                base.readDataFromXml(reader);
        }

        public override bool IsClicked(Point pos)
        {
            Point[] posArr = { pos };
            Matrix transformMatrix = new Matrix();
            transformMatrix.RotateAt(angle, center); // Somehow it has to be rotated at angle instead of -angle like in rotationMatrix...
            transformMatrix.TransformPoints(posArr);
            pos = posArr[0];
            return pos.X > location.X - ALLOWED_ERROR && pos.X < bottomRight.X + ALLOWED_ERROR && pos.Y > location.Y - ALLOWED_ERROR && pos.Y < bottomRight.Y + ALLOWED_ERROR;
        }
    }
    
    /// <summary>Represents a layer that contains two coordinates</summary>
    abstract class LayerTwoPoint : Layer
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerTwoPoint(Point loc, Point loc2, Color col) : base(loc, col)
        { secondLocation = loc2; }

        /// <summary>The second location of the layer</summary>
        protected Point secondLocation;
        /// <summary>Property to get or set the second location of the layer</summary>
        public Point SecondLocation
        {
            get { return secondLocation; }
            set { secondLocation = value; }
        }

        /// <summary>Returns the bounding rectangle of this layer</summary>
        /// <returns>The bounding rectangle of this layer</returns>
        public Rectangle GetBounds()
        {
            return new Rectangle(
                new Point(Math.Min(location.X, secondLocation.X), Math.Min(location.Y, secondLocation.Y)),
                new Size(Math.Abs(location.X - secondLocation.X), Math.Abs(location.Y - secondLocation.Y)));
        }
        
        /// <summary>Rotates the layer</summary>
        /// <param name="xCenter">The x center to rotate around</param>
        /// <param name="yCenter">The y center to rotate around</param>
        public override void Rotate(double xCenter, double yCenter)
        {
            double localX = location.X - xCenter;
            double localY = location.Y - yCenter;
            double secondLocalX = secondLocation.X - xCenter;
            double secondLocalY = secondLocation.Y - yCenter;

            location.X = (int)(xCenter + localY);
            location.Y = (int)(yCenter - localX);
            secondLocation.X = (int)(xCenter + secondLocalY);
            secondLocation.Y = (int)(yCenter - secondLocalX);
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-two-point";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        /// <summary>Writes the data that compose this layer to a XML document.</summary>
        /// <param name="writer">The XML document to write to</param>
        protected override void writeDataToXml(XmlWriter writer)
        {
            // Call the base method
            base.writeDataToXml(writer);

            // The second location
            writer.WriteStartElement("second-location");
            writer.WriteStartElement("x");
            writer.WriteValue(secondLocation.X);
            writer.WriteEndElement();
            writer.WriteStartElement("y");
            writer.WriteValue(secondLocation.Y);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        /// <summary>Called when the current node in the XmlReader should be parsed into data for this layer</summary>
        /// <param name="reader">The XmlReader that holds the current node</param>
        protected override void readDataFromXml(XmlReader reader)
        {
            // Read the second location
            if(reader.Name == "second-location")
            {
                reader.Read();
                while(reader.NodeType != XmlNodeType.EndElement || reader.Name != "second-location")
                {
                    if(reader.NodeType == XmlNodeType.Element && (reader.Name == "x" || reader.Name == "y"))
                    {
                        if(reader.Name == "x")
                            secondLocation.X = reader.ReadElementContentAsInt();
                        else
                            secondLocation.Y = reader.ReadElementContentAsInt();
                    }
                }
            }
            // Let the base class read its data
            else
                base.readDataFromXml(reader);
        }
    }

    /// <summary>Represents a layer that contains a straight line</summary>
    class LayerLine : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerLine(Point loc, Point loc2, Color col) : base(loc, loc2, col)
        { }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawLine(pen, location, secondLocation);
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-line";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        /// <summary>Calculates the distance from a point to a line (note: to a LINE not a line segment)</summary>
        /// <param name="l1">The first point describing the line</param>
        /// <param name="l2">The second point describing the line</param>
        /// <param name="pos">The point</param>
        /// <returns>The distance from the point to the line</returns>
        public static double DistanceToLine(Point l1, Point l2, Point pos)
        {
            // If it's a vertical line
            if(l1.X == l2.X)
            {
                // Check if `pos` is vertically in between the two points
                if(pos.Y <= Math.Max(l1.Y, l2.Y) && pos.Y >= Math.Min(l1.Y, l2.Y))
                    return Math.Abs(pos.X - l1.X);                                          // Then the shortest distance is perfectly horizontal
                else
                {
                    int dy = Math.Min(Math.Abs(l1.Y - pos.Y), Math.Abs(l2.Y - pos.Y));
                    return Math.Sqrt((pos.X - l1.X) * (pos.X - l1.X) + dy * dy);            // Pythagorean theorem
                }
            }

            // Next is an implementation of the 'other possible equation' found at
            // http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Another_possible_equation
            Point leftPoint = l1.X < l2.X ? l1 : l2;
            Point rightPoint = leftPoint == l1 ? l2 : l1;
            double slope = (double) (rightPoint.Y - leftPoint.Y) / (double) (rightPoint.X - leftPoint.X);   // The slope of the line between l1 and l2
            double intercept = (double)(leftPoint.Y - slope * leftPoint.X);                                 // The intercept with x = 0 of the line through l1 and l2
            double xIntercept = (double)((pos.X + slope * pos.Y - slope * intercept) / (Math.Pow(slope, 2) + 1));
            return Math.Sqrt(Math.Pow(xIntercept - pos.X, 2) + Math.Pow(slope*xIntercept + intercept - pos.Y, 2));
        }

        public override bool IsClicked(Point pos)
        {
            RectangleF bounds = GetBounds();
            bounds.Inflate(ALLOWED_ERROR + 1.5f, ALLOWED_ERROR + 1.5f);
            return bounds.Contains(pos) && DistanceToLine(location, secondLocation, pos) < ALLOWED_ERROR + 1.5;
        }
    }

    /// <summary>Represents a layer that contains a filled rectangle</summary>
    class LayerRectFilled : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerRectFilled(Point loc, Point loc2, Color col)
            : base(loc, loc2, col)
        { }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(color), GetBounds());
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-rect-filled";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        public override bool IsClicked(Point pos)
        {
            Rectangle bounds = GetBounds();
            bounds.Inflate(ALLOWED_ERROR, ALLOWED_ERROR);
            return bounds.Contains(pos);
        }
    }

    /// <summary>Represents a layer that contains a bitmap</summary>
    class LayerBitmap : LayerRectFilled
    {
        private Bitmap bitmap;

        /// <summary>Constructor</summary>
        /// <param name="loc1">The location</param>
        /// <param name="bitmap">The bitmap it contains</param>
        public LayerBitmap(Point loc1, Bitmap bitmap) : base(loc1, new Point(), Color.Black)
        {
            secondLocation = new Point(loc1.X + bitmap.Width, loc1.Y + bitmap.Height);
            this.bitmap = bitmap;
        }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            g.DrawImage(bitmap, GetBounds());
        }

        /// <summary>Rotates the layer</summary>
        /// <param name="xCenter">The center in x to rotate around</param>
        /// <param name="yCenter">The center in y to rotate around</param>
        public override void Rotate(double xCenter, double yCenter)
        {
            this.bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            base.Rotate(xCenter, yCenter);
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-image";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        /// <summary>Writes the data that compose this layer to a XML document.</summary>
        /// <param name="writer">The XML document to write to</param>
        protected override void writeDataToXml(XmlWriter writer)
        {
            // Call the base method
            base.writeDataToXml(writer);

            // Let's get a saveable format of the image
            ImageConverter icer = new ImageConverter();
            String imageData = Convert.ToBase64String((byte[])icer.ConvertTo(bitmap, typeof(byte[])));

            // The image itself
            writer.WriteStartElement("image-data");
            writer.WriteCData(imageData);
            writer.WriteEndElement();
        }

        /// <summary>Called when the current node in the XmlReader should be parsed into data for this layer</summary>
        /// <param name="reader">The XmlReader that holds the current node</param>
        protected override void readDataFromXml(XmlReader reader)
        {
            // Read the second location
            if (reader.Name == "image-data")
            {
                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "image-data")
                {
                    if (reader.NodeType == XmlNodeType.CDATA)
                    {
                        ImageConverter icer = new ImageConverter();
                        String imageString = reader.ReadContentAsString();
                        byte[] imageData = Convert.FromBase64String(imageString);
                        bitmap = (Bitmap)icer.ConvertFrom(imageData);
                    }
                }
            }
            // Let the base class read its data
            else
                base.readDataFromXml(reader);
        }
    }

    /// <summary>Represents a layer that contains an open rectangle</summary>
    class LayerRectOpen : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerRectOpen(Point loc, Point loc2, Color col)
            : base(loc, loc2, col)
        { }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawRectangle(pen, GetBounds());
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-rect-open";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        public override bool IsClicked(Point pos)
        {
            // Check if `pos` is within the border (correct for the border thickness)
            RectangleF bounds = GetBounds();
            bounds.Inflate(ALLOWED_ERROR + 1.5f, ALLOWED_ERROR + 1.5f);
            if(!bounds.Contains(pos)) return false;

            // Check if `pos` is inside the rectangle (i.e. not on the border)
            bounds.Inflate(-2 * ALLOWED_ERROR - 3, -2 * ALLOWED_ERROR - 3);
            return !bounds.Contains(pos);
        }
    }

    /// <summary>Represents a layer that contains a filled ellipse</summary>
    class LayerEllipseFilled : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerEllipseFilled(Point loc, Point loc2, Color col)
            : base(loc, loc2, col)
        { }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(color), GetBounds());
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-ellipse-filled";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        public static bool IsInEllipse(double rx, double ry, double x, double y)
        {
            // The equation of an ellipse (with its center on the origin) is: x^2/rx^2 + y^2/ry^2 = 1
            // So for a point (px, py) inside the ellipse we see that px^2/rx^2 + py^2/ry^2 <= 1
            return x * x / (rx * rx) + y * y / (ry * ry) <= 1.0;
        }

        public override bool IsClicked(Point pos)
        {
            // Calculate the coordinates of the center of the ellipse
            double centerX = (secondLocation.X - location.X) / 2.0 + location.X;
            double centerY = (secondLocation.Y - location.Y) / 2.0 + location.Y;

            // Return whether `pos` is within the ellipse (which we grow by ALLOWED_ERROR in each direction to allow for a small error)
            return IsInEllipse(GetBounds().Width / 2.0 + ALLOWED_ERROR, GetBounds().Height / 2.0 + ALLOWED_ERROR, pos.X - centerX, pos.Y - centerY);
        }
    }

    /// <summary>Represents a layer that contains an open ellipse</summary>
    class LayerEllipseOpen : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerEllipseOpen(Point loc, Point loc2, Color col)
            : base(loc, loc2, col)
        { }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawEllipse(pen, GetBounds());
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-ellipse-open";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        public override bool IsClicked(Point pos)
        {
            // Calculate the coordinates of the center of the ellipse
            double centerX = (secondLocation.X - location.X) / 2.0 + location.X;
            double centerY = (secondLocation.Y - location.Y) / 2.0 + location.Y;

            // Determine whether `pos` is within the ellipse which we grow by 4.0 in each direction for 2 reasons:
            //  1. The line thickness of the ellipse is 1.5
            //  2. We add ALLOWED_ERROR to allow for a small error
            if(LayerEllipseFilled.IsInEllipse(GetBounds().Width / 2.0 + 1.5 + ALLOWED_ERROR, GetBounds().Height / 2.0 + 1.5 + ALLOWED_ERROR, pos.X - centerX, pos.Y - centerY))
            {
                // If `pos` is within the ellipse after shrinking it by (1.5 + ALLOWED_ERROR) (the same amount as we grew it),
                // then `pos` is not on the border of the ellipse (i.e. not close enough)
                if(LayerEllipseFilled.IsInEllipse(GetBounds().Width / 2.0 - 1.5 - ALLOWED_ERROR, GetBounds().Height / 2.0 - 1.5 - ALLOWED_ERROR, pos.X - centerX, pos.Y - centerY))
                    return false;
                return true;
            }
            return false;
        }
    }

    /// <summary>Represents a layer that contains a path</summary>
    class LayerPath : Layer
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="col">The color</param>
        public LayerPath(Point loc, Color col) : base(loc, col)
        { }

        /// <summary>All points in the path (`location` is the first point and is not in this list)</summary>
        protected List<Point> points = new List<Point>();
        /// <summary>Property to manage the points in the path (`location` is the first point and is not in this list)</summary>
        public List<Point> Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            // If there are no points in the path, there is nothing to do
            if(points.Count == 0) return;

            // Create the path
            GraphicsPath path = new GraphicsPath();
            Point[] pathPoints = new Point[points.Count + 1];
            pathPoints[0] = location;
            Array.Copy(points.ToArray(), 0, pathPoints, 1, points.Count);
            path.AddLines(pathPoints);

            // Draw the path
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawPath(pen, path);
        }

        /// <summary>Static property to get the XML name of this type of layer</summary>
        public new const String XML_NAME = "layer-path";
        /// <summary>Property to get the XML name of this type of layer</summary>
        public override String XmlName { get { return XML_NAME; } }

        /// <summary>Writes the data that compose this layer to a XML document.</summary>
        /// <param name="writer">The XML document to write to</param>
        protected override void writeDataToXml(XmlWriter writer)
        {
            // Call the base method
            base.writeDataToXml(writer);

            // The points
            writer.WriteStartElement("points");
            writer.WriteStartAttribute("count");
            writer.WriteValue(points.Count);
            writer.WriteEndAttribute();
            foreach(Point p in points)
            {
                writer.WriteStartElement("point");
                writer.WriteStartElement("x");
                writer.WriteValue(p.X);
                writer.WriteEndElement();
                writer.WriteStartElement("y");
                writer.WriteValue(p.Y);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>Called when the current node in the XmlReader should be parsed into data for this layer</summary>
        /// <param name="reader">The XmlReader that holds the current node</param>
        protected override void readDataFromXml(XmlReader reader)
        {
            // Read the points
            if(reader.Name == "points")
            {
                // Read the amount of points
                int pointCount = 0;
                while(reader.MoveToNextAttribute())
                {
                    if(reader.Name == "count")
                        pointCount = reader.ReadContentAsInt();
                }

                // Create the list of points
                points = new List<Point>(pointCount);
                while(reader.NodeType != XmlNodeType.EndElement || reader.Name != "points")
                {
                    reader.Read();
                    if(reader.NodeType == XmlNodeType.Element && reader.Name == "point")
                    {
                        Point p = new Point(0, 0);
                        reader.Read();
                        while(reader.NodeType != XmlNodeType.EndElement || reader.Name != "point")
                        {
                            if(reader.NodeType == XmlNodeType.Element && (reader.Name == "x" || reader.Name == "y"))
                            {
                                if(reader.Name == "x")
                                    p.X = reader.ReadElementContentAsInt();
                                else
                                    p.Y = reader.ReadElementContentAsInt();
                            }
                        }
                        points.Add(p);
                    }
                }
            }
            // Let the base class read its data
            else
                base.readDataFromXml(reader);
        }

        public override bool IsClicked(Point pos)
        {
            // Create an array containing all the points in the path
            Point[] pathPoints = new Point[points.Count + 1];
            pathPoints[0] = location;
            Array.Copy(points.ToArray(), 0, pathPoints, 1, points.Count);

            // Loop through all the line segments and check if `pos` is close enough to one of them
            for(int i = 0; i < pathPoints.Length - 1; ++i)
            {
                RectangleF bounds = new RectangleF(
                    new Point(Math.Min(pathPoints[i].X, pathPoints[i + 1].X), Math.Min(pathPoints[i].Y, pathPoints[i + 1].Y)),
                    new Size(Math.Abs(pathPoints[i].X - pathPoints[i + 1].X), Math.Abs(pathPoints[i].Y - pathPoints[i + 1].Y)));
                bounds.Inflate(ALLOWED_ERROR + 1.5f, ALLOWED_ERROR + 1.5f);
                if(bounds.Contains(pos) && LayerLine.DistanceToLine(pathPoints[i], pathPoints[i + 1], pos) < ALLOWED_ERROR + 1.5)
                    return true;
            }

            // No line segment found that is close enough, so we return false
            return false;
        }

        public override void Rotate(double xCenter, double yCenter)
        {
            double locLocalX = location.X - xCenter;
            double locLocalY = location.Y - yCenter;

            location.X = (int)(xCenter + locLocalY);
            location.Y = (int)(yCenter - locLocalX);

            for (int i = 0; i < Points.Count; i++)
            {
                double localX = Points[i].X - xCenter;
                double localY = Points[i].Y - yCenter;

                Points[i] = new Point((int)(xCenter + localY), (int)(yCenter - localX));
            }        
        }
    }
}
