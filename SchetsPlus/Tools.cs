﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    interface ISchetsTool
    {
        /// <summary>Called when the mouse button is pressed</summary>
        /// <param name="s">The SchetsControls that the button is pressed on</param>
        /// <param name="p">The location of the cursor</param>
        void MuisVast(SchetsControl s, Point p);

        /// <summary>Called when the mouse is dragging (with the mouse button down)</summary>
        /// <param name="s">The SchetsControls that the button is being dragged on</param>
        /// <param name="p">The location of the cursor</param>
        void MuisDrag(SchetsControl s, Point p);

        /// <summary>Called when the mouse button is released</summary>
        /// <param name="s">The SchetsControls that the button is released from</param>
        /// <param name="p">The location of the cursor</param>
        void MuisLos(SchetsControl s, Point p);

        /// <summary>Called when a key is pressed</summary>
        /// <param name="s">The SchetsControls that the key is pressed on</param>
        /// <param name="c">The key that is pressed</param>
        void Letter(SchetsControl s, char c);

        /// <summary>Whether or not the tool is currently editting a layer</summary>
        /// <returns>True if a layer is currently being editted, false otherwise</returns>
        bool IsEditting();
    }

    abstract class StartpuntTool : ISchetsTool
    {
        /// <summary>The point where this tool started editting</summary>
        protected Point startpunt;
        /// <summary>The color that is to be used with this tool</summary>
        protected Color color;
        /// <summary>The layer we're currently editting (or null if we're not editting a layer)</summary>
        protected Layer edittingLayer = null;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            color = s.PenKleur;
            startpunt = p;
            edittingLayer = null;
        }
        public abstract void MuisLos(SchetsControl s, Point p);
        public abstract void MuisDrag(SchetsControl s, Point p);
        public abstract void Letter(SchetsControl s, char c);

        public virtual bool IsEditting()
        { return edittingLayer != null; }
    }

    class TekstTool : StartpuntTool
    {
        public override string ToString() { return "tekst"; }

        public override void MuisVast(SchetsControl s, Point p)
        {
            if(edittingLayer != null)
            {
                ((LayerText) edittingLayer).Editting = false;
                s.CommitAction(new SchetsActionAddLayer(edittingLayer));
                s.Invalidate();
            }

            base.MuisVast(s, p);
        }
        public override void MuisLos(SchetsControl s, Point p) { }
        public override void MuisDrag(SchetsControl s, Point p) { }

        public override void Letter(SchetsControl s, char c)
        {
            if (c >= 32)
            {
                if(edittingLayer == null)
                {
                    s.Schets.Layers.Add(edittingLayer = new LayerText(this.startpunt, color, new String(c, 1)));
                    ((LayerText) edittingLayer).Editting = true;
                }
                else
                    ((LayerText) edittingLayer).Text += c;
                s.Invalidate();
            }
        }
    }

    abstract class TweepuntTool : StartpuntTool
    {
        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(
                new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y)),
                new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y)) );
        }

        public override void MuisDrag(SchetsControl s, Point p)
        { Bezig(s, startpunt, p); }

        public override void MuisLos(SchetsControl s, Point p)
        {
            Bezig(s, startpunt, p);
            if(edittingLayer != null)
            {
                s.CommitAction(new SchetsActionAddLayer(edittingLayer));
                edittingLayer = null;
            }
        }

        // Ignore any key presses
        public override void Letter(SchetsControl s, char c){ }

        /// <summary>Called when a new layer should be created</summary>
        /// <param name="p1">The first point/location of the layer</param>
        /// <param name="p2">The second point/location of the layer</param>
        /// <returns>The created layer</returns>
        public abstract LayerTwoPoint CreateLayer(Point p1, Point p2);

        /// <summary>Called when the tool is busy drawing the layer</summary>
        /// <param name="s">The SchetsControl that the layer should be added to</param>
        /// <param name="p1">The first point/location of the layer</param>
        /// <param name="p2">The second point/location of the layer</param>
        public virtual void Bezig(SchetsControl s, Point p1, Point p2)
        {
            if(edittingLayer == null)
                s.Schets.Layers.Add(edittingLayer = CreateLayer(p1, p2));
            else
                ((LayerTwoPoint) edittingLayer).SecondLocation = p2;
            s.Invalidate();
        }
    }

    class RechthoekTool : TweepuntTool
    {
        public override string ToString() { return "kader"; }

        public override LayerTwoPoint CreateLayer(Point p1, Point p2)
        { return new LayerRectOpen(p1, p2, color); }
    }
    
    class VolRechthoekTool : RechthoekTool
    {
        public override string ToString() { return "vlak"; }

        public override LayerTwoPoint CreateLayer(Point p1, Point p2)
        { return new LayerRectFilled(p1, p2, color); }
    }

    class CircleTool : TweepuntTool
    {
        public override string ToString() { return "rondje"; }

        public override LayerTwoPoint CreateLayer(Point p1, Point p2)
        { return new LayerCircleOpen(p1, p2, color); }
    }

    class FilledCircleTool : CircleTool
    {
        public override string ToString() { return "cirkel"; }

        public override LayerTwoPoint CreateLayer(Point p1, Point p2)
        { return new LayerCircleFilled(p1, p2, color); }
    }

    class LijnTool : TweepuntTool
    {
        public override string ToString() { return "lijn"; }

        public override LayerTwoPoint CreateLayer(Point p1, Point p2)
        { return new LayerLine(p1, p2, color); }
    }

    class PenTool : StartpuntTool
    {
        public override string ToString() { return "pen"; }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            if(edittingLayer == null)
            {
                edittingLayer = new LayerPath(startpunt, color);
                ((LayerPath) edittingLayer).Points.Add(p);
                s.Schets.Layers.Add(edittingLayer);
            }
            else
                ((LayerPath) edittingLayer).Points.Add(p);
            s.Invalidate();
        }

        // Commit the action on a mouse release event
        public override void MuisLos(SchetsControl s, Point p)
        {
            if(edittingLayer != null)
            {
                s.CommitAction(new SchetsActionAddLayer(edittingLayer));
                edittingLayer = null;
            }
        }

        // Ignore any key presses
        public override void Letter(SchetsControl s, char c) { }
    }
    
    class GumTool : PenTool
    {
        public override string ToString() { return "gum"; }
    }
}
