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

        /// <summary>Construct the action</summary>
        /// <param name="layer">The layer that was removed from the SchetsControl</param>
        public SchetsActionRemoveLayer(Layer layer)
        { removedLayer = layer; }

        /// <summary>Undo the action for the given SchetsControl</summary>
        /// <param name="s">The SchetsControl that the action should be undone for</param>
        public override void Undo(SchetsControl s)
        {
            s.Schets.Layers.Add(removedLayer);
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
}
