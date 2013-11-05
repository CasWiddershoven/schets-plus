﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager = new ResourceManager("SchetsEditor.Properties.Resources", Assembly.GetExecutingAssembly());

        /// <summary>The undo button</summary>
        Button buttonUndo;
        /// <summary>The redo button</summary>
        Button buttonRedo;

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool.ToolChange(schetscontrol);
            this.huidigeTool = (ISchetsTool) ((ToolStripMenuItem) obj).Tag;
            this.huidigeTool.ToolSelected(schetscontrol);
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool.ToolChange(schetscontrol);
            this.huidigeTool = (ISchetsTool) ((RadioButton) obj).Tag;
            this.huidigeTool.ToolSelected(schetscontrol);
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool(),
                                      new LijnTool(),
                                      new RechthoekTool(),
                                      new VolRechthoekTool(),
                                      new EllipseTool(),
                                      new FilledEllipseTool(),
                                      new TekstTool(),
                                      new GumTool() };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan" 
                                 };

            this.ClientSize = new Size(700, 500);
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {
                                           if(mea.Button != MouseButtons.Left) return;
                                           vast = true;
                                           huidigeTool.MuisVast(schetscontrol, mea.Location);
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       {
                                           if(vast)
                                               huidigeTool.MuisDrag(schetscontrol, mea.Location);
                                       };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
                                       {
                                           if(mea.Button != MouseButtons.Left) return;
                                           vast = false;
                                           huidigeTool.MuisLos(schetscontrol, mea.Location);
                                       };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
                                       {
                                           huidigeTool.Letter(schetscontrol, kpea.KeyChar);
                                       };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
            this.FormClosing += SchetsWin_FormClosing;
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add(new ToolStripSeparator());
            menu.DropDownItems.Add("Openen", null, loadFile);
            menu.DropDownItems.Add("Opslaan", null, saveFile);
            menu.DropDownItems.Add("Importeren van...", null, loadBitmap);
            menu.DropDownItems.Add("Exporteren naar...", null, saveBitmap);
            menu.DropDownItems.Add(new ToolStripSeparator());
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach(ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image) resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach(string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach(ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image) resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if(t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetscontrol.Schoon;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += schetscontrol.Roteer;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = "Penkleur:";
            l.Location = new Point(180, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach(string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);

            // Create the undo and redo buttons
            buttonUndo = new Button();
            buttonUndo.Text = "Undo";
            buttonUndo.Location = new Point(400, 0);
            buttonUndo.Click += (object o, EventArgs ea) => { callUndo(); };
            buttonUndo.Enabled = false;
            paneel.Controls.Add(buttonUndo);

            buttonRedo = new Button();
            buttonRedo.Text = "Redo";
            buttonRedo.Location = new Point(490, 0);
            buttonRedo.Click += (object o, EventArgs ea) => { callRedo(); };
            buttonRedo.Enabled = false;
            paneel.Controls.Add(buttonRedo);

            schetscontrol.CanUndoRedo += (bool canUndo, bool canRedo) =>
                {
                    buttonUndo.Enabled = canUndo;
                    buttonRedo.Enabled = canRedo;
                };
        }

        /// <summary>Calls the Undo() method on the SchetsControl</summary>
        private void callUndo()
        {
            // Do no undo / redo if we're currently editting a layer
            if(!huidigeTool.IsEditting())
                schetscontrol.Undo();
        }

        /// <summary>Calls the Redo() method on the SchetsControl</summary>
        private void callRedo()
        {
            // Do no undo / redo if we're currently editting a layer
            if(!huidigeTool.IsEditting())
                schetscontrol.Redo();
        }

        // Override ProcessCmdKey to enable shortcut keys (like Ctrl+Z)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Handle the shortcut keys
            if(keyData == (Keys.Control | Keys.Z))
            {
                callUndo();
                return true;
            }
            else if(keyData == (Keys.Control | Keys.Y))
            {
                callRedo();
                return true;
            }

            // No shortcut found, so we call the parent's handler
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>Asks the user if he's sure he wants to continue with the current operation because some unsaved changes will be lost.</summary>
        /// <returns>Whether or not the user wants to continue with the current operation</returns>
        private bool askAboutUnsavedChanges()
        {
            return  DialogResult.Yes == MessageBox.Show("Weet u zeker dat u door wilt gaan?\nEr zijn onopgeslagen wijzigingen die verloren zullen gaan!",
                                                        "Onopgeslagen wijzigingen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        // Fires when the form is about to be closed
        void SchetsWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Notify the user of unsaved changes (if there are any)
            if(!schetscontrol.ChangesSaved && !askAboutUnsavedChanges())
                e.Cancel = true;
        }

        /// <summary>A function to get a file name using an OpenFileDialog</summary>
        /// <param name="filetype">The type of the file expected by the caller</param>
        /// <returns>The filename, or an empty string if something went wrong</returns>
        private string getFileNameOpen(string filetype)
        {
            // Create an open file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = filetype;

            // Show the dialog
            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;
            return "";
        }

        /// <summary>A function to get a file name using a SaveFileDialog</summary>
        /// <param name="filetype">The type of the file expected by the caller</param>
        /// <returns>A tuple containing the filename (or an empty string if something went wrong) and the selected filter index</returns>
        private Tuple<string, int> getFileNameSave(string filetype)
        {
            // Create an open file dialog
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = filetype;

            // Show the dialog
            if (dlg.ShowDialog() == DialogResult.OK)
                return new Tuple<string, int>(dlg.FileName, dlg.FilterIndex);
            return new Tuple<string, int>("", -1);
        }
        
        // Event handler to load the current drawing from a file
        private void loadFile(object obj, EventArgs ea)
        {
            // Notify the user of unsaved changes (if there are any)
            if(!schetscontrol.ChangesSaved && !askAboutUnsavedChanges())
                return;

            string filename = getFileNameOpen("SchetsPlus schets (*.schets)|*.schets");

            if(filename != "")
            {
                try
                {
                    schetscontrol.Schets.LoadFromFile(filename);
                    schetscontrol.ClearHistory();
                    schetscontrol.Invalidate();
                }
                catch(Exception e)
                {
                    MessageBox.Show("Er is een fout opgetreden bij het laden van het bestand.\nFoutboodschap:\n\n" + e.Message, "FOUT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } 
        }

        // Event handler to load the current drawing from a bitmap
        private void loadBitmap(object obj, EventArgs ea)
        {
            // Notify the user of unsaved changes (if there are any)
            if (!schetscontrol.ChangesSaved && !askAboutUnsavedChanges())
                return;

            string filename = getFileNameOpen("Afbeeldingsbestanden (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp");

            if (filename != "")
            {
                try
                {
                    schetscontrol.Schets.LoadBitmap(filename);
                    schetscontrol.ClearHistory();
                    schetscontrol.Invalidate();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Er is een fout opgetreden bij het laden van het bestand.\nFoutboodschap:\n\n" + e.Message, "FOUT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Event handler to save the current drawing to a file
        private void saveFile(object obj, EventArgs ea)
        {
            Tuple<string, int> result = getFileNameSave("SchetsPlus schets (*.schets)|*.schets");

            if(result.Item1 != "")
            {
                try
                {
                    schetscontrol.Schets.SaveToFile(result.Item1);
                    schetscontrol.ChangesSaved = true;
                }
                catch(Exception e)
                {
                    MessageBox.Show("Er is een fout opgetreden bij het opslaan van het bestand.\nFoutboodschap:\n\n" + e.Message, "FOUT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Event handler to save the current drawing to a bitmap
        private void saveBitmap(object obj, EventArgs ea)
        {
            Tuple<string, int> result = getFileNameSave("PNG Afbeelding (*.png)|*.png|JPEG Afbeelding (*.jpg;*.jpeg)|*.jpg;*.jpeg|GIF Afbeelding (*.gif)|*.gif|Bitmap afbeelding (*.bmp)|*.bmp");

            if(result.Item1 != "")
            {
                try
                {
                    ImageFormat format = ImageFormat.Png;
                    switch(result.Item2)
                    {
                        case 2: format = ImageFormat.Jpeg; break;
                        case 3: format = ImageFormat.Gif; break;
                        case 4: format = ImageFormat.Bmp; break;
                    }
                    schetscontrol.Schets.saveBitmap(result.Item1, schetscontrol.Width, schetscontrol.Height, format);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Er is een fout opgetreden bij het opslaan van het bestand.\nFoutboodschap:\n\n" + e.Message, "FOUT!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
