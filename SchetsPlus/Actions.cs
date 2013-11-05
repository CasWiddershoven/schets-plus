using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchetsEditor
{
    /// <summary>Represents an action can be undone / redone</summary>
    abstract class SchetsAction
    {
        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public abstract void Undo(SchetsControl s);

        /// <summary>Redo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be redone for</param>
        public abstract void Redo(SchetsControl s);
    }

    /// <summary>Represents an action where a layer is removed</summary>
    class SchetsActionRemoveLayer : SchetsAction
    {
        /// <summary>The layer that was removed from the SchetsControl</summary>
        private Layer removedLayer;

        /// <summary>The index of the layer that was removed</summary>
        private int index;

        /// <summary>Construct the action</summary>
        /// <param name="layer">The layer that was removed from the SchetsControl</param>
        /// <param name="i">The index of the layer</param>
        public SchetsActionRemoveLayer(Layer layer, int i)
        {
            removedLayer = layer;
            index = i;
        }

        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public override void Undo(SchetsControl s)
        {
            s.Schets.Layers.Insert(index, removedLayer);
        }

        /// <summary>Redo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be redone for</param>
        public override void Redo(SchetsControl s)
        {
            s.Schets.Layers.Remove(removedLayer);
        }
    }

    /// <summary>Represents an action where a layer is added</summary>
    class SchetsActionAddLayer : SchetsAction
    {
        /// <summary>The layer that was added to the SchetsControl</summary>
        private Layer addedLayer;

        /// <summary>Construct the action</summary>
        /// <param name="layer">The layer that is added to the SchetsControl</param>
        public SchetsActionAddLayer(Layer layer)
        { addedLayer = layer;  }

        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public override void Undo(SchetsControl s)
        {
            s.Schets.Layers.Remove(addedLayer);
        }

        /// <summary>Redo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be redone for</param>
        public override void Redo(SchetsControl s)
        {
            s.Schets.Layers.Add(addedLayer);
        }
    }
    
    /// <summary>Represents an action where the entire drawing is cleared</summary>
    class SchetsActionClear : SchetsAction
    {
        /// <summary>The layers at the moment the drawing was cleared</summary>
        private List<Layer> layers;

        /// <summary>Construct the action</summary>
        /// <param name="l">The layers at the moment the drawing was cleared</param>
        public SchetsActionClear(List<Layer> l)
        { layers = l;  }

        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public override void Undo(SchetsControl s)
        {
            s.Schets.Layers.Clear();
            s.Schets.Layers.InsertRange(0, layers);
        }

        /// <summary>Redo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be redone for</param>
        public override void Redo(SchetsControl s)
        {
            s.Schets.Layers.Clear();
        }
    }

    /// <summary>Represents an action where the entire drawing is rotated 90 degrees</summary>
    class SchetsActionRotate : SchetsAction
    {
        /// <summary>The x center at the time of rotating</summary>
        private double xCenter;
        /// <summary>The y center at the time of rotating</summary>
        private double yCenter;

        /// <summary>Constructor</summary>
        /// <param name="xc">The x center</param>
        /// <param name="yc">The y center</param>
        public SchetsActionRotate(double xc, double yc)
        {
            xCenter = xc;
            yCenter = yc;
        }

        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public override void Undo(SchetsControl s)
        {
            // Three times 90 degrees is the same as one time -90 degrees
            for (int i = 0; i < 3; i++)
            {
                foreach (Layer layer in s.Schets.Layers)
                    layer.Rotate(xCenter, yCenter);
            }
        }

        /// <summary>Redo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be redone for</param>
        public override void Redo(SchetsControl s)
        {
            foreach (Layer layer in s.Schets.Layers)
                layer.Rotate(xCenter, yCenter);
        }
    }
}
