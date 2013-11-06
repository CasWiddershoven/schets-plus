using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SchetsEditor
{
    class SchetsControl : UserControl
    {
        /// <summary>The canvas that we're drawing on</summary>
        private Schets schets;
        /// <summary>Property to get the canvas</summary>
        public Schets Schets { get { return schets; } }

        /// <summary>The list of actions that have been committed to this SchetsControl</summary>
        private List<SchetsAction> actions = new List<SchetsAction>();
        /// <summary>The current position in the action list (i.e. the index of the action that was last committed)</summary>
        private int actionPos = -1;

        /// <summary>The current color that is to be used by the tools</summary>
        private Color penkleur;
        /// <summary>Property to get the current color that is to be used by the tools</summary>
        public Color PenKleur { get { return penkleur; } }

        /// <summary>The current pen width that is to be used by the tools</summary>
        private float penWidth = 3.0f;
        /// <summary>Property to get the current pen width that is to be used by the tools</summary>
        public float PenWidth
        {
            get { return penWidth; }
            set { penWidth = Math.Max(1.0f, value); }
        }

        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.BackColor = Color.White;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.ResizeRedraw = true;
            this.DoubleBuffered = true;
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }
        public void Schoon(object o, EventArgs ea)
        {
            CommitAction(new SchetsActionClear(new List<Layer>(schets.Layers)));
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {
            CommitAction(new SchetsActionRotate(Width / 2.0, Height / 2.0));
            schets.Roteer(Width / 2.0, Height / 2.0);
            this.Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {
            changeColor(((ComboBox) obj).Text);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            changeColor(((ToolStripMenuItem) obj).Text);
        }

        /// <summary>The color dialog that's used in this SchetsControl (made it a member variable so it can remember previous selected colors)</summary>
        private ColorDialog colorDlg = null;

        /// <summary>Changes the current color to the color with the given name</summary>
        /// <param name="colorName">The (English) name of the color or "Other" to show a color dialog</param>
        private void changeColor(String colorName)
        {
            if(colorName != "Other")
                penkleur = Color.FromName(colorName);
            else
            {
                if(colorDlg == null)
                    colorDlg = new ColorDialog();
                if(colorDlg.ShowDialog() == DialogResult.OK)
                    penkleur = colorDlg.Color;
            }
        }

        /// <summary>Clears the history, this will also set ChangesSaved to true (since there are no more changes)</summary>
        public void ClearHistory()
        {
            // Clear the history and reset actionPos
            actions.Clear();
            actionPos = -1;
            
            // There are no changes anymore, so all changes are now saved
            ChangesSaved = true;

            // Trigger the CanUndoRedo event
            callCanUndoRedo();
        }

        /// <summary>Adds an action to the action list</summary>
        /// <param name="action">The action to add</param>
        public void CommitAction(SchetsAction action)
        {
            // Remove any undone actions
            if(actionSavedPos >= actionPos + 1)
                actionSavedPos = -2;
            if(actionPos + 1 < actions.Count)
                actions.RemoveRange(actionPos + 1, actions.Count - actionPos - 1);

            // Add the action
            actions.Add(action);
            actionPos = actions.Count - 1;

            // Trigger the CanUndoRedo event
            callCanUndoRedo();
        }

        /// <summary>Undo the last action</summary>
        /// <returns>True if there is another action that can be undone, false otherwise</returns>
        public bool Undo()
        {
            if(actionPos >= 0)
            {
                actions[actionPos--].Undo(this);
                callCanUndoRedo();
                Invalidate();
            }
            return actionPos >= 0;
        }

        /// <summary>Redo the last undone action</summary>
        /// <returns>True if there is another action that can be redone, false otherwise</returns>
        public bool Redo()
        {
            if(actionPos + 1 < actions.Count)
            {
                actions[++actionPos].Redo(this);
                callCanUndoRedo();
                Invalidate();
            }
            return actionPos + 1 < actions.Count;
        }

        /// <summary>The handler for the CanUndoRedo event</summary>
        /// <param name="canUndo">Whether there are actions that can be undone</param>
        /// <param name="canRedo">Whether there are actions that can be redone</param>
        public delegate void CanUndoRedoHandler(bool canUndo, bool canRedo);

        /// <summary>The CanUndoRedo event, called when a situation changes in whether there actions that can be undone/redone</summary>
        public event CanUndoRedoHandler CanUndoRedo;
        
        /// <summary>Trigger the CanUndoRedo event</summary>
        private void callCanUndoRedo()
        {
            if(CanUndoRedo != null)
                CanUndoRedo(actionPos >= 0, actionPos + 1 < actions.Count);
        }

        /// <summary>The position in the action list for which the changes where saved, or -2 if there is no position for which the changes are saved</summary>
        private int actionSavedPos = -1;

        /// <summary>Whether or not the changes are saved</summary>
        public bool ChangesSaved
        {
            get { return actionPos == actionSavedPos; }
            set { actionSavedPos = value ? actionPos : -2; }
        }

        /// <summary>An enum describing the possible actions for the context menu</summary>
        public enum ReorderActions
        { SendToTop, OneUp, OneDown, SendToBottom }

        /// <summary>Change the order of the layers by changing the z-index of the given layer</summary>
        /// <param name="layer">The layer whose z-index is to be changed</param>
        /// <param name="action">How the z-index of the layer should be changed</param>
        public void ChangeLayerOrder(Layer layer, ReorderActions action)
        {
            // If no layer is given, stop here
            if(layer == null) return;

            // Find the current index of the layer and determine it's new index
            int currIndex = Schets.Layers.IndexOf(layer);
            int newIndex = currIndex;
            switch(action)
            {
                case ReorderActions.SendToTop:
                    if(newIndex == Schets.Layers.Count - 1) return;
                    newIndex = Schets.Layers.Count - 1;
                    break;

                case ReorderActions.OneUp:
                    if(newIndex == Schets.Layers.Count - 1) return;
                    ++newIndex;
                    break;

                case ReorderActions.OneDown:
                    if(newIndex == 0) return;
                    --newIndex;
                    break;

                case ReorderActions.SendToBottom:
                    if(newIndex == 0) return;
                    newIndex = 0;
                    break;
            }

            // Change the z-index of the layer
            Schets.Layers.RemoveAt(currIndex);
            Schets.Layers.Insert(newIndex, layer);

            // Commit the action
            CommitAction(new SchetsActionReorder(currIndex, newIndex));

            // Redraw
            Invalidate();
        }
    }
}
