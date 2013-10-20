using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    class Schets
    {
        /// <summary>The layers that form the image</summary>
        private List<Layer> layers = new List<Layer>();

        /// <summary>Property to get the layers</summary>
        public List<Layer> Layers { get { return layers; } }
        
        /// <summary>Resize the canvas</summary>
        /// <param name="sz">New canvas size</param>
        public void VeranderAfmeting(Size sz)
        {
            // Implement me
        }

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
    }
}
