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

        /// <summary>Whether the layer is clicked or not when clicking at the given position</summary>
        /// <param name="pos">The position where user clicked</param>
        /// <returns>True if the layer is clicked, otherwise false</returns>
        //public abstract bool IsClicked(Point pos);
        // To be used for the eraser tool (and possibly a tool to select and move a layer)
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
            }
            g.DrawString(text, font, new SolidBrush(color), location);
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
    }

    /// <summary>Represents a layer that contains a filled rectangle</summary>
    class LayerRectFilled : LayerTwoPoint
    {
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerRectFilled(Point loc, Point loc2, Color col) : base(loc, loc2, col)
        { }

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
        /// <summary>Constructor</summary>
        /// <param name="loc">The location</param>
        /// <param name="loc2">The second location</param>
        /// <param name="col">The color</param>
        public LayerRectOpen(Point loc, Point loc2, Color col) : base(loc, loc2, col)
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
