namespace SpreadsheetGUI
{
    partial class Form1
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
            if (disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBoxCellName = new System.Windows.Forms.TextBox();
            this.labelCellName = new System.Windows.Forms.Label();
            this.labelCellValue = new System.Windows.Forms.Label();
            this.labelCellContents = new System.Windows.Forms.Label();
            this.textBoxCellValue = new System.Windows.Forms.TextBox();
            this.textBoxCellContents = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.NewStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripNightModeButton = new System.Windows.Forms.ToolStripButton();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxCellName
            // 
            this.textBoxCellName.Location = new System.Drawing.Point(136, 96);
            this.textBoxCellName.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCellName.Name = "textBoxCellName";
            this.textBoxCellName.ReadOnly = true;
            this.textBoxCellName.Size = new System.Drawing.Size(100, 31);
            this.textBoxCellName.TabIndex = 1;
            // 
            // labelCellName
            // 
            this.labelCellName.AutoSize = true;
            this.labelCellName.Location = new System.Drawing.Point(12, 102);
            this.labelCellName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCellName.Name = "labelCellName";
            this.labelCellName.Size = new System.Drawing.Size(117, 25);
            this.labelCellName.TabIndex = 2;
            this.labelCellName.Text = "Cell Name:";
            // 
            // labelCellValue
            // 
            this.labelCellValue.AutoSize = true;
            this.labelCellValue.Location = new System.Drawing.Point(248, 102);
            this.labelCellValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCellValue.Name = "labelCellValue";
            this.labelCellValue.Size = new System.Drawing.Size(73, 25);
            this.labelCellValue.TabIndex = 3;
            this.labelCellValue.Text = "Value:";
            // 
            // labelCellContents
            // 
            this.labelCellContents.AutoSize = true;
            this.labelCellContents.Location = new System.Drawing.Point(784, 102);
            this.labelCellContents.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCellContents.Name = "labelCellContents";
            this.labelCellContents.Size = new System.Drawing.Size(104, 25);
            this.labelCellContents.TabIndex = 4;
            this.labelCellContents.Text = "Contents:";
            // 
            // textBoxCellValue
            // 
            this.textBoxCellValue.Location = new System.Drawing.Point(330, 96);
            this.textBoxCellValue.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCellValue.Name = "textBoxCellValue";
            this.textBoxCellValue.ReadOnly = true;
            this.textBoxCellValue.Size = new System.Drawing.Size(442, 31);
            this.textBoxCellValue.TabIndex = 5;
            // 
            // textBoxCellContents
            // 
            this.textBoxCellContents.Location = new System.Drawing.Point(896, 96);
            this.textBoxCellContents.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxCellContents.Name = "textBoxCellContents";
            this.textBoxCellContents.Size = new System.Drawing.Size(484, 31);
            this.textBoxCellContents.TabIndex = 6;
            this.textBoxCellContents.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCellContents_KeyPress);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.toolStripNightModeButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1632, 42);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewStripMenuItem,
            this.saveToolStripMenuItem,
            this.fileToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(54, 36);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // NewStripMenuItem
            // 
            this.NewStripMenuItem.Name = "NewStripMenuItem";
            this.NewStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.NewStripMenuItem.Text = "New";
            this.NewStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.fileToolStripMenuItem.Text = "Open";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(208, 44);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripNightModeButton
            // 
            this.toolStripNightModeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripNightModeButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripNightModeButton.Image")));
            this.toolStripNightModeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNightModeButton.Name = "toolStripNightModeButton";
            this.toolStripNightModeButton.Size = new System.Drawing.Size(46, 36);
            this.toolStripNightModeButton.Text = "Night Mode";
            this.toolStripNightModeButton.Click += new System.EventHandler(this.toolStripNightModeButton_Click);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Location = new System.Drawing.Point(1418, 92);
            this.buttonHelp.Margin = new System.Windows.Forms.Padding(6);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(150, 44);
            this.buttonHelp.TabIndex = 8;
            this.buttonHelp.Text = "Help";
            this.buttonHelp.UseVisualStyleBackColor = true;
            this.buttonHelp.Click += new System.EventHandler(this.buttonHelp_Click);
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel1.Location = new System.Drawing.Point(18, 152);
            this.spreadsheetPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(1612, 704);
            this.spreadsheetPanel1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1632, 877);
            this.Controls.Add(this.buttonHelp);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.textBoxCellContents);
            this.Controls.Add(this.textBoxCellValue);
            this.Controls.Add(this.labelCellContents);
            this.Controls.Add(this.labelCellValue);
            this.Controls.Add(this.labelCellName);
            this.Controls.Add(this.textBoxCellName);
            this.Controls.Add(this.spreadsheetPanel1);
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Untitled";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing_1);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.TextBox textBoxCellName;
        private System.Windows.Forms.Label labelCellName;
        private System.Windows.Forms.Label labelCellValue;
        private System.Windows.Forms.Label labelCellContents;
        private System.Windows.Forms.TextBox textBoxCellValue;
        private System.Windows.Forms.TextBox textBoxCellContents;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem NewStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.HelpProvider helpProvider2;
        private System.Windows.Forms.ToolStripButton toolStripNightModeButton;
    }
}

