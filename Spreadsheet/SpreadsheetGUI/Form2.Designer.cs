namespace SpreadsheetGUI
{
    partial class Form2
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
            this.LabelUserName = new System.Windows.Forms.Label();
            this.LabelAddress = new System.Windows.Forms.Label();
            this.TextBoxUserName = new System.Windows.Forms.TextBox();
            this.TextBoxAddress = new System.Windows.Forms.TextBox();
            this.ButtonConnect = new System.Windows.Forms.Button();
            this.TextBoxSpreadsheetName = new System.Windows.Forms.TextBox();
            this.ButtonSelect = new System.Windows.Forms.Button();
            this.LabelSpreadsheet = new System.Windows.Forms.Label();
            this.DisplaySheets = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // LabelUserName
            // 
            this.LabelUserName.AutoSize = true;
            this.LabelUserName.Location = new System.Drawing.Point(34, 16);
            this.LabelUserName.Name = "LabelUserName";
            this.LabelUserName.Size = new System.Drawing.Size(58, 13);
            this.LabelUserName.TabIndex = 0;
            this.LabelUserName.Text = "Username:";
            // 
            // LabelAddress
            // 
            this.LabelAddress.AutoSize = true;
            this.LabelAddress.Location = new System.Drawing.Point(34, 50);
            this.LabelAddress.Name = "LabelAddress";
            this.LabelAddress.Size = new System.Drawing.Size(82, 13);
            this.LabelAddress.TabIndex = 1;
            this.LabelAddress.Text = "Server Address:";
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.Location = new System.Drawing.Point(149, 13);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.Size = new System.Drawing.Size(232, 20);
            this.TextBoxUserName.TabIndex = 2;
            // 
            // TextBoxAddress
            // 
            this.TextBoxAddress.Location = new System.Drawing.Point(149, 47);
            this.TextBoxAddress.Name = "TextBoxAddress";
            this.TextBoxAddress.Size = new System.Drawing.Size(232, 20);
            this.TextBoxAddress.TabIndex = 3;
            // 
            // ButtonConnect
            // 
            this.ButtonConnect.Location = new System.Drawing.Point(418, 11);
            this.ButtonConnect.Name = "ButtonConnect";
            this.ButtonConnect.Size = new System.Drawing.Size(75, 23);
            this.ButtonConnect.TabIndex = 4;
            this.ButtonConnect.Text = "Connect";
            this.ButtonConnect.UseVisualStyleBackColor = true;
            this.ButtonConnect.Click += new System.EventHandler(this.ButtonConnect_Click);
            // 
            // TextBoxSpreadsheetName
            // 
            this.TextBoxSpreadsheetName.Enabled = false;
            this.TextBoxSpreadsheetName.Location = new System.Drawing.Point(149, 286);
            this.TextBoxSpreadsheetName.Name = "TextBoxSpreadsheetName";
            this.TextBoxSpreadsheetName.Size = new System.Drawing.Size(232, 20);
            this.TextBoxSpreadsheetName.TabIndex = 5;
            // 
            // ButtonSelect
            // 
            this.ButtonSelect.Enabled = false;
            this.ButtonSelect.Location = new System.Drawing.Point(418, 284);
            this.ButtonSelect.Name = "ButtonSelect";
            this.ButtonSelect.Size = new System.Drawing.Size(75, 23);
            this.ButtonSelect.TabIndex = 6;
            this.ButtonSelect.Text = "Select ";
            this.ButtonSelect.UseVisualStyleBackColor = true;
            this.ButtonSelect.Click += new System.EventHandler(this.ButtonSelect_Click);
            // 
            // LabelSpreadsheet
            // 
            this.LabelSpreadsheet.AutoSize = true;
            this.LabelSpreadsheet.Location = new System.Drawing.Point(42, 289);
            this.LabelSpreadsheet.Name = "LabelSpreadsheet";
            this.LabelSpreadsheet.Size = new System.Drawing.Size(101, 13);
            this.LabelSpreadsheet.TabIndex = 7;
            this.LabelSpreadsheet.Text = "Spreadsheet Name:";
            // 
            // DisplaySheets
            // 
            this.DisplaySheets.Location = new System.Drawing.Point(149, 73);
            this.DisplaySheets.Multiline = true;
            this.DisplaySheets.Name = "DisplaySheets";
            this.DisplaySheets.ReadOnly = true;
            this.DisplaySheets.Size = new System.Drawing.Size(344, 207);
            this.DisplaySheets.TabIndex = 8;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 318);
            this.Controls.Add(this.DisplaySheets);
            this.Controls.Add(this.LabelSpreadsheet);
            this.Controls.Add(this.ButtonSelect);
            this.Controls.Add(this.TextBoxSpreadsheetName);
            this.Controls.Add(this.ButtonConnect);
            this.Controls.Add(this.TextBoxAddress);
            this.Controls.Add(this.TextBoxUserName);
            this.Controls.Add(this.LabelAddress);
            this.Controls.Add(this.LabelUserName);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelUserName;
        private System.Windows.Forms.Label LabelAddress;
        private System.Windows.Forms.TextBox TextBoxUserName;
        private System.Windows.Forms.TextBox TextBoxAddress;
        private System.Windows.Forms.Button ButtonConnect;
        private System.Windows.Forms.TextBox TextBoxSpreadsheetName;
        private System.Windows.Forms.Button ButtonSelect;
        private System.Windows.Forms.Label LabelSpreadsheet;
        private System.Windows.Forms.TextBox DisplaySheets;
    }
}