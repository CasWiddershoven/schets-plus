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
            schets.Roteer();
            Invalidate();
        }
        public void VeranderKleur(object obj, EventArgs ea)
        {
            string kleurNaam = ((ComboBox) obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }
        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            string kleurNaam = ((ToolStripMenuItem) obj).Text;
            penkleur = Color.FromName(kleurNaam);
        }

        /// <summary>Adds an action to the action list</summary>
        /// <param name="action">The action to add</param>
        public void CommitAction(SchetsAction action)
        {
            // Remove any undone actions
            if(actionPos + 1 < actions.Count)
                actions.RemoveRange(actionPos + 1, actions.Count - actionPos - 1);

            // Add the action
            actions.Add(action);
            actionPos = actions.Count - 1;
        }

        /// <summary>Undo the last action</summary>
        /// <returns>True if there is another action that can be undone, false otherwise</returns>
        public bool Undo()
        {
            if(actionPos >= 0)
            {
                actions[actionPos--].Undo(this);
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
                Invalidate();
            }
            return actionPos + 1 < actions.Count;
        }
    }
}
