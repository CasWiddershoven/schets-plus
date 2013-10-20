using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    class SchetsControl : UserControl
    {
        /// <summary>The canvas that we're drawing on</summary>
        private Schets schets;
        private Color penkleur;

        /// <summary>Property to get the canvas</summary>
        public Schets Schets { get { return schets; } }

        public Color PenKleur
        {
            get { return penkleur; }
        }
        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.BackColor = Color.White;
            this.schets = new Schets();
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
            this.DoubleBuffered = true;
            this.veranderAfmeting(null, null);
        }
        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }
        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }
        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }
        public void Roteer(object o, EventArgs ea)
        {
            schets.Roteer();
            this.veranderAfmeting(o, ea);
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
    }
}
