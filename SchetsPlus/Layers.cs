using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    /// <summary>Represents a single layer in the drawing</summary>
    abstract class Layer
    {
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

        /// <summary>Whether the layer is clicked or not when clicking at the given position</summary>
        /// <param name="pos">The position where user clicked</param>
        /// <returns>True if the layer is clicked, otherwise false</returns>
        //public abstract bool IsClicked(Point pos);
    }

    /// <summary>Represents a layer that contains some text</summary>
    class LayerText : Layer
    {
        /// <summary>The text that is to be displayed</summary>
        protected String text;
        /// <summary>Property to set or get the text that is to be displayed</summary>
        public String Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Font font = new Font("Tahoma", 40);
            g.DrawString(text, font, new SolidBrush(color), location);
        }
    }
    
    /// <summary>Represents a layer that contains two coordinates</summary>
    abstract class LayerTwoPoint : Layer
    {
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
    }

    /// <summary>Represents a layer that contains a straight line</summary>
    class LayerLine : LayerTwoPoint
    {
        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawLine(pen, location, secondLocation);
        }
    }

    /// <summary>Represents a layer that contains a filled rectangle</summary>
    class LayerRectFilled : LayerTwoPoint
    {
        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(color), GetBounds());
        }
    }

    /// <summary>Represents a layer that contains an open rectangle</summary>
    class LayerRectOpen : LayerTwoPoint
    {
        /// <summary>Draws the layer</summary>
        /// <param name="g">The graphics object that is to be used to draw the layer</param>
        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(color, 3);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawRectangle(pen, GetBounds());
        }
    }

    /// <summary>Represents a layer that contains a path</summary>
    class LayerPath : Layer
    {
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
            if(points.Count == 0) return ;

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
    }
}
