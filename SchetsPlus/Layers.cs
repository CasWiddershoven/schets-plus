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
                    color = Color.FromArgb(reader.ReadContentAsInt());
                // else: ERROR
                
                //if(reader.NodeType != XmlNodeType.EndElement || reader.Name != "color"): ERROR
            }
        }

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
            Font font = new Font("Tahoma", 40);
            if(editting)
            {
                SizeF textSize = g.MeasureString(text, font);
                g.DrawRectangle(new Pen(Color.Gray, 3), location.X, location.Y, textSize.Width, textSize.Height);
                bottomRight = new Point(location.X + (int)textSize.Width, location.Y + (int)textSize.Height);
            }
            g.DrawString(text, font, new SolidBrush(color), location);
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
                // else: ERROR

                //if(reader.NodeType != XmlNodeType.EndElement || reader.Name != "text"): ERROR
            }
            // Let the base class read its data
            else
                base.readDataFromXml(reader);
        }

        public override bool IsClicked(Point pos)
        {
            const int maxDistance = 5;
            return pos.X > location.X - maxDistance && pos.X < bottomRight.X + maxDistance && pos.Y > location.Y - maxDistance && pos.Y < bottomRight.Y + maxDistance;
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

        /// <summary>Calculates the distance from a point to a line</summary>
        /// <param name="pos">The point</param>
        /// <returns>The distance from the point to the line</returns>
        private double distance(Point pos)
        {
            double slope;
            Point leftPoint = location.X < secondLocation.X ? location : secondLocation;
            Point rightPoint = leftPoint == location ? secondLocation : location;
            try
            {
                slope = (double)(rightPoint.Y - leftPoint.Y) / (double)(rightPoint.X - leftPoint.X); // The slope of the line between location and secondLocation
            }
            catch (DivideByZeroException) // If it's a vertical line
            {
                Point highPoint, lowPoint;
                if (location.Y < secondLocation.Y) // location is higher than secondLocation
                {
                    highPoint = location;
                    lowPoint = secondLocation;
                }
                else
                {
                    highPoint = secondLocation;
                    lowPoint = location;
                }
                if (pos.Y < lowPoint.Y && pos.Y > highPoint.Y) // pos is inbetween the two points
                {
                    return Math.Abs(pos.X - lowPoint.X); // So the shortest distance is perfectly horizontal
                }
                else
                { // pos is higher or lower than the line
                    int yDistance;
                    if (pos.Y < highPoint.Y)
                    { // pos is higher than the line
                        yDistance = highPoint.Y - pos.Y;
                    }
                    else
                    { // pos is lower than the line
                        yDistance = pos.Y - lowPoint.Y;
                    }
                    // Pythagorean theorem
                    return Math.Sqrt(Math.Pow(Math.Abs(pos.X - lowPoint.X), 2) + Math.Pow(yDistance, 2));
                }
            }
            double intercept = (double)(leftPoint.Y - slope * leftPoint.X); // The intercept with x=0 of the line through location and secondLocation

            // Next is an implementation of the 'other possible equation' found at
            // http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Another_possible_equation
            double xIntercept = (double)((pos.X + slope * pos.Y - slope * intercept) / (Math.Pow(slope, 2) + 1));
            double distance = Math.Sqrt(Math.Pow(xIntercept - pos.X, 2) + Math.Pow(slope*xIntercept + intercept - pos.Y, 2));

            return distance;
        }

        public override bool IsClicked(Point pos)
        {
            const double maxDistance = 5.0;
            int left = location.X < secondLocation.X ? location.X : secondLocation.X;
            int right = location.X == left ? secondLocation.X : location.X;
            int top = location.Y < secondLocation.Y ? location.Y : secondLocation.Y;
            int bottom = location.Y == top ? secondLocation.Y : location.Y;
            return !(left - pos.X > maxDistance ||
                    pos.X - right > maxDistance ||
                    top - pos.Y > maxDistance ||
                    pos.Y - bottom > maxDistance || 
                    distance(pos) > maxDistance);
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
            int left = location.X < secondLocation.X ? location.X : secondLocation.X;
            int right = location.X == left ? secondLocation.X : location.X;
            int top = location.Y < secondLocation.Y ? location.Y : secondLocation.Y;
            int bottom = location.Y == top ? secondLocation.Y : location.Y;
            double maxDistance = 5;
            if (    pos.X < left - maxDistance || 
                    pos.X > right + maxDistance || 
                    pos.Y < top - maxDistance || 
                    pos.Y > bottom + maxDistance)
            {
                return false;
            }
            return true;
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
            int left = location.X < secondLocation.X ? location.X : secondLocation.X;
            int right = location.X == left ? secondLocation.X : location.X;
            int top = location.Y < secondLocation.Y ? location.Y : secondLocation.Y;
            int bottom = location.Y == top ? secondLocation.Y : location.Y;
            double maxDistance = 5;
            if (    ((Math.Abs(pos.X - left) < maxDistance) || 
                    (Math.Abs(pos.X - right) < maxDistance) && 
                    pos.Y - maxDistance > top && 
                    pos.Y + maxDistance < bottom) ||
                    ((Math.Abs(pos.Y - top) < maxDistance) ||
                    (Math.Abs(pos.Y - bottom) < maxDistance) &&
                    pos.X - maxDistance > left &&
                    pos.X + maxDistance < right)) // Or; if it's within the boundaries of the lines of the rectangle
            {
                return true;
            }
            return false;
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

        public override bool IsClicked(Point pos)
        {
            /*double centerX = (double)(location.X + secondLocation.X) / 2d;
            double centerY = (double)(location.Y + secondLocation.Y) / 2d;
            double radius = (double)*/
            return false;
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

        /// <summary>Calculates the distance from a point to a line</summary>
        /// <param name="pos">The point</param>
        /// <returns>The distance from the point to the line</returns>
        private double distance(Point pos, Point location, Point secondLocation)
        {
            double slope;
            try
            {
                slope = (double)((secondLocation.Y - location.Y) / (location.X - secondLocation.X)); // The slope of the line between location and secondLocation
            }
            catch (DivideByZeroException e) // If it's a vertical line
            {
                Point highPoint, lowPoint;
                if (location.Y < secondLocation.Y) // location is higher than secondLocation
                {
                    highPoint = location;
                    lowPoint = secondLocation;
                } else {
                    highPoint = secondLocation;
                    lowPoint = location;
                }
                if (pos.Y < lowPoint.Y && pos.Y > highPoint.Y) // pos is inbetween the two points
                {
                    return Math.Abs(pos.X - lowPoint.X); // So the shortest distance is perfectly horizontal
                } else { // pos is higher or lower than the line
                    int yDistance;
                    if (pos.Y < highPoint.Y) { // pos is higher than the line
                        yDistance = highPoint.Y - pos.Y;
                    } else { // pos is lower than the line
                        yDistance = pos.Y - lowPoint.Y;
                    }
                    // Pythagorean theorem
                    return Math.Sqrt(Math.Pow(Math.Abs(pos.X - lowPoint.X), 2) + Math.Pow(yDistance, 2));
                }
            }
            double intercept = (double)(location.Y - slope * location.X); // The intercept with x=0 of the line through location and secondLocation

            // Next is an implementation of the 'other possible equation' found at
            // http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Another_possible_equation
            double xIntercept = (double)((pos.X + slope * pos.Y - slope * intercept) / (Math.Pow(slope, 2) + 1));
            double distance = Math.Sqrt(Math.Pow(xIntercept - pos.X, 2) + Math.Pow(xIntercept + intercept - pos.Y, 2));

            return distance;
        }

        public override bool IsClicked(Point pos)
        {
            int maxDistance = 5;
            Point[] pathPoints = new Point[points.Count + 1];
            pathPoints[0] = location;
            Array.Copy(points.ToArray(), 0, pathPoints, 1, points.Count);
            for (int i = 0; i < pathPoints.Length - 1; i++)
            {
                if (distance(pos, pathPoints[i], pathPoints[i + 1]) < maxDistance)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
