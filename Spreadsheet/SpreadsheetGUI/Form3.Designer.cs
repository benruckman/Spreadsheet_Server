namespace SpreadsheetGUI
{
    partial class Form3
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
            this.DisplaySpreadsheetNames = new System.Windows.Forms.TextBox();
            this.InputSpreadsheetName = new System.Windows.Forms.TextBox();
            this.InputSpreadsheetNameLabel = new System.Windows.Forms.Label();
            this.ListOfSpreadsheetsLabel = new System.Windows.Forms.Label();
            this.SpreadsheetEntered = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DisplaySpreadsheetNames
            // 
            this.DisplaySpreadsheetNames.Location = new System.Drawing.Point(56, 74);
            this.DisplaySpreadsheetNames.Multiline = true;
            this.DisplaySpreadsheetNames.Name = "DisplaySpreadsheetNames";
            this.DisplaySpreadsheetNames.ReadOnly = true;
            this.DisplaySpreadsheetNames.Size = new System.Drawing.Size(732, 364);
            this.DisplaySpreadsheetNames.TabIndex = 0;
            this.DisplaySpreadsheetNames.Text = "Testing \\ntesting hello there";
            // 
            // InputSpreadsheetName
            // 
            this.InputSpreadsheetName.Location = new System.Drawing.Point(56, 24);
            this.InputSpreadsheetName.Name = "InputSpreadsheetName";
            this.InputSpreadsheetName.Size = new System.Drawing.Size(573, 20);
            this.InputSpreadsheetName.TabIndex = 1;
            // 
            // InputSpreadsheetNameLabel
            // 
            this.InputSpreadsheetNameLabel.AutoSize = true;
            this.InputSpreadsheetNameLabel.Location = new System.Drawing.Point(53, 9);
            this.InputSpreadsheetNameLabel.Name = "InputSpreadsheetNameLabel";
            this.InputSpreadsheetNameLabel.Size = new System.Drawing.Size(555, 13);
            this.InputSpreadsheetNameLabel.TabIndex = 2;
            this.InputSpreadsheetNameLabel.Text = "Enter spreadsheet name here to connect to new spreadsheet, or enter a different n" +
    "ame to create a new spreadsheet";
            // 
            // ListOfSpreadsheetsLabel
            // 
            this.ListOfSpreadsheetsLabel.AutoSize = true;
            this.ListOfSpreadsheetsLabel.Location = new System.Drawing.Point(56, 51);
            this.ListOfSpreadsheetsLabel.Name = "ListOfSpreadsheetsLabel";
            this.ListOfSpreadsheetsLabel.Size = new System.Drawing.Size(183, 13);
            this.ListOfSpreadsheetsLabel.TabIndex = 3;
            this.ListOfSpreadsheetsLabel.Text = "List of Spreadsheets already in server";
            // 
            // SpreadsheetEntered
            // 
            this.SpreadsheetEntered.Location = new System.Drawing.Point(635, 9);
            this.SpreadsheetEntered.Name = "SpreadsheetEntered";
            this.SpreadsheetEntered.Size = new System.Drawing.Size(153, 35);
            this.SpreadsheetEntered.TabIndex = 4;
            this.SpreadsheetEntered.Text = "Open";
            this.SpreadsheetEntered.UseVisualStyleBackColor = true;
            this.SpreadsheetEntered.Click += new System.EventHandler(this.OpenSpreadsheetButtonPressed);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SpreadsheetEntered);
            this.Controls.Add(this.ListOfSpreadsheetsLabel);
            this.Controls.Add(this.InputSpreadsheetNameLabel);
            this.Controls.Add(this.InputSpreadsheetName);
            this.Controls.Add(this.DisplaySpreadsheetNames);
            this.Name = "Form3";
            this.Text = "Form3";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox DisplaySpreadsheetNames;
        private System.Windows.Forms.TextBox InputSpreadsheetName;
        private System.Windows.Forms.Label InputSpreadsheetNameLabel;
        private System.Windows.Forms.Label ListOfSpreadsheetsLabel;
        private System.Windows.Forms.Button SpreadsheetEntered;
    }
}