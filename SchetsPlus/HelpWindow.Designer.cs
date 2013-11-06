namespace SchetsEditor
{
    partial class HelpWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label labelNewDrawingHeader;
            System.Windows.Forms.Label labelNewDrawingText;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpWindow));
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
            System.Windows.Forms.Label labelDrawingHeader;
            System.Windows.Forms.Label labelDrawingText;
            System.Windows.Forms.Label labelReorderHeader;
            System.Windows.Forms.Label labelReorderingText;
            System.Windows.Forms.Label labelSaveLoadHeader;
            System.Windows.Forms.Label labelUndoRedoHeader;
            System.Windows.Forms.Label labelHelpHeader;
            this.labelSaveLoadText = new System.Windows.Forms.Label();
            this.labelUndoRedoText = new System.Windows.Forms.Label();
            labelNewDrawingHeader = new System.Windows.Forms.Label();
            labelNewDrawingText = new System.Windows.Forms.Label();
            tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            labelDrawingHeader = new System.Windows.Forms.Label();
            labelDrawingText = new System.Windows.Forms.Label();
            labelReorderHeader = new System.Windows.Forms.Label();
            labelReorderingText = new System.Windows.Forms.Label();
            labelSaveLoadHeader = new System.Windows.Forms.Label();
            labelUndoRedoHeader = new System.Windows.Forms.Label();
            labelHelpHeader = new System.Windows.Forms.Label();
            tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelNewDrawingHeader
            // 
            labelNewDrawingHeader.AutoSize = true;
            labelNewDrawingHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelNewDrawingHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelNewDrawingHeader.Location = new System.Drawing.Point(3, 28);
            labelNewDrawingHeader.Name = "labelNewDrawingHeader";
            labelNewDrawingHeader.Size = new System.Drawing.Size(568, 16);
            labelNewDrawingHeader.TabIndex = 0;
            labelNewDrawingHeader.Text = "Nieuwe schets";
            // 
            // labelNewDrawingText
            // 
            labelNewDrawingText.AutoSize = true;
            labelNewDrawingText.Dock = System.Windows.Forms.DockStyle.Fill;
            labelNewDrawingText.Location = new System.Drawing.Point(3, 44);
            labelNewDrawingText.Name = "labelNewDrawingText";
            labelNewDrawingText.Size = new System.Drawing.Size(568, 39);
            labelNewDrawingText.TabIndex = 1;
            labelNewDrawingText.Text = resources.GetString("labelNewDrawingText.Text");
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(labelNewDrawingHeader, 0, 2);
            tableLayoutPanel.Controls.Add(labelNewDrawingText, 0, 3);
            tableLayoutPanel.Controls.Add(labelDrawingHeader, 0, 5);
            tableLayoutPanel.Controls.Add(labelDrawingText, 0, 6);
            tableLayoutPanel.Controls.Add(labelReorderHeader, 0, 8);
            tableLayoutPanel.Controls.Add(labelReorderingText, 0, 9);
            tableLayoutPanel.Controls.Add(labelSaveLoadHeader, 0, 14);
            tableLayoutPanel.Controls.Add(this.labelSaveLoadText, 0, 15);
            tableLayoutPanel.Controls.Add(labelUndoRedoHeader, 0, 11);
            tableLayoutPanel.Controls.Add(this.labelUndoRedoText, 0, 12);
            tableLayoutPanel.Controls.Add(labelHelpHeader, 0, 0);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 16;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel.Size = new System.Drawing.Size(574, 476);
            tableLayoutPanel.TabIndex = 2;
            // 
            // labelDrawingHeader
            // 
            labelDrawingHeader.AutoSize = true;
            labelDrawingHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelDrawingHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelDrawingHeader.Location = new System.Drawing.Point(3, 91);
            labelDrawingHeader.Name = "labelDrawingHeader";
            labelDrawingHeader.Size = new System.Drawing.Size(568, 16);
            labelDrawingHeader.TabIndex = 2;
            labelDrawingHeader.Text = "Tekenen";
            // 
            // labelDrawingText
            // 
            labelDrawingText.AutoSize = true;
            labelDrawingText.Dock = System.Windows.Forms.DockStyle.Fill;
            labelDrawingText.Location = new System.Drawing.Point(3, 107);
            labelDrawingText.Name = "labelDrawingText";
            labelDrawingText.Size = new System.Drawing.Size(568, 65);
            labelDrawingText.TabIndex = 3;
            labelDrawingText.Text = resources.GetString("labelDrawingText.Text");
            // 
            // labelReorderHeader
            // 
            labelReorderHeader.AutoSize = true;
            labelReorderHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelReorderHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelReorderHeader.Location = new System.Drawing.Point(3, 180);
            labelReorderHeader.Name = "labelReorderHeader";
            labelReorderHeader.Size = new System.Drawing.Size(568, 16);
            labelReorderHeader.TabIndex = 4;
            labelReorderHeader.Text = "De volgorde van de vormpjes wijzigen";
            // 
            // labelReorderingText
            // 
            labelReorderingText.AutoSize = true;
            labelReorderingText.Dock = System.Windows.Forms.DockStyle.Fill;
            labelReorderingText.Location = new System.Drawing.Point(3, 196);
            labelReorderingText.Name = "labelReorderingText";
            labelReorderingText.Size = new System.Drawing.Size(568, 52);
            labelReorderingText.TabIndex = 5;
            labelReorderingText.Text = resources.GetString("labelReorderingText.Text");
            // 
            // labelSaveLoadHeader
            // 
            labelSaveLoadHeader.AutoSize = true;
            labelSaveLoadHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelSaveLoadHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelSaveLoadHeader.Location = new System.Drawing.Point(3, 345);
            labelSaveLoadHeader.Name = "labelSaveLoadHeader";
            labelSaveLoadHeader.Size = new System.Drawing.Size(568, 16);
            labelSaveLoadHeader.TabIndex = 6;
            labelSaveLoadHeader.Text = "Opslaan, laden en overige acties";
            // 
            // labelSaveLoadText
            // 
            this.labelSaveLoadText.AutoSize = true;
            this.labelSaveLoadText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSaveLoadText.Location = new System.Drawing.Point(3, 361);
            this.labelSaveLoadText.Name = "labelSaveLoadText";
            this.labelSaveLoadText.Size = new System.Drawing.Size(568, 115);
            this.labelSaveLoadText.TabIndex = 8;
            this.labelSaveLoadText.Text = resources.GetString("labelSaveLoadText.Text");
            // 
            // labelUndoRedoHeader
            // 
            labelUndoRedoHeader.AutoSize = true;
            labelUndoRedoHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelUndoRedoHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelUndoRedoHeader.Location = new System.Drawing.Point(3, 256);
            labelUndoRedoHeader.Name = "labelUndoRedoHeader";
            labelUndoRedoHeader.Size = new System.Drawing.Size(568, 16);
            labelUndoRedoHeader.TabIndex = 9;
            labelUndoRedoHeader.Text = "Undo en redo";
            // 
            // labelUndoRedoText
            // 
            this.labelUndoRedoText.AutoSize = true;
            this.labelUndoRedoText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelUndoRedoText.Location = new System.Drawing.Point(3, 272);
            this.labelUndoRedoText.Name = "labelUndoRedoText";
            this.labelUndoRedoText.Size = new System.Drawing.Size(568, 65);
            this.labelUndoRedoText.TabIndex = 10;
            this.labelUndoRedoText.Text = resources.GetString("labelUndoRedoText.Text");
            // 
            // labelHelpHeader
            // 
            labelHelpHeader.AutoSize = true;
            labelHelpHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            labelHelpHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            labelHelpHeader.Location = new System.Drawing.Point(3, 0);
            labelHelpHeader.Name = "labelHelpHeader";
            labelHelpHeader.Size = new System.Drawing.Size(568, 20);
            labelHelpHeader.TabIndex = 11;
            labelHelpHeader.Text = "SchetsPlus - Help";
            labelHelpHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // HelpWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 476);
            this.Controls.Add(tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpWindow";
            this.Text = "Help";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelSaveLoadText;
        private System.Windows.Forms.Label labelUndoRedoText;

    }
}