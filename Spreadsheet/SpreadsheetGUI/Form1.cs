using Newtonsoft.Json;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public delegate void TextBoxContentsChangedHandler(SpreadsheetPanel sender);

    public partial class Form1 : Form
    {
        NetworkControl NC;

        /// <summary>
        /// Creates a new window displaying an empty spreadsheet
        /// </summary>
        public Form1(NetworkControl NetC, string filename)
        {

            this.NC = NetC;

            NC.Update += ServerUpdate;
       
            InitializeComponent();

            // the name of the form
            this.Text = filename;

            // highlights 
            this.ActiveControl = textBoxCellContents;

            // set up listener for panel selection changed
            spreadsheetPanel1.SelectionChanged += OnSelectionChanged;

            // set initial selection to A1, 1
            spreadsheetPanel1.SetSelection(0, 0);

            // call the method to update selection
            OnSelectionChanged(spreadsheetPanel1);

            // set the name of the window to the filename
            Text = filename;
        }

        /// <summary>
        /// Process JSon coming from the server, and update the spreadsheet
        /// </summary>
        /// <param name="message"></param>
        private void ServerUpdate(SS.MessageType message)
        {
            if (message.type.Equals("cellUpdated"))
            {
                int col = spreadsheetPanel1.GetCellNameCol(message.cellName);
                int row = spreadsheetPanel1.GetCellNameRow(message.cellName);
                spreadsheetPanel1.SetContents(col, row, message.cellContents);
                spreadsheetPanel1.GetValue(col, row, out string val);
            }

            if (message.type.Equals("cellSelected"))
            {
                int col = spreadsheetPanel1.GetCellNameCol(message.cellName);
                int row = spreadsheetPanel1.GetCellNameRow(message.cellName);
                spreadsheetPanel1.ChangeUserSelection(col, row, message.selectorID); //Might want to actually send the username, but might want to keep it this way, not sure yet
            }

            if (message.type.Equals("requestError"))
            {
                MessageBox.Show("Request error: " + message.errorMessage);
            }

            if(message.type.Equals("disconnected"))
            {
                // remove disconnected user from our list
            }

            if (message.type.Equals("serverError"))
            {
                MessageBox.Show("Server error: " + message.errorMessage);
            }

            return;
        }

        /// <summary>
        /// when the selection of a spreadsheet is changed, updates values of name, contents, and value of spreadsheet. 
        /// 
        /// </summary>
        /// <param name="sender"></param>
        private void OnSelectionChanged(SpreadsheetPanel sender)
        {
            // Get where we are in the spreadsheet
            sender.GetSelection(out int col, out int row);

            // Update name textBox
            textBoxCellName.Text = (ConvertCellName(col, row));

            // Update value textBox
            sender.GetValue(col, row, out string val);
            textBoxCellValue.Text = val;

            // Update contents textBox
            textBoxCellContents.Text = sender.GetContents(col, row);

            // Focus the input onto the contents textbox
            textBoxCellContents.Focus();

            // Sending selection changed to server via JSon
            RequestTypeSelection r = new RequestTypeSelection("selectCell", spreadsheetPanel1.ConvertCellName(col, row));
            string request = JsonConvert.SerializeObject(r) + "\n";
            NC.SendData(request);
        }

        /// <summary>
        /// Converts a cell name to coordinates 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string ConvertCellName(int col, int row)
        {
            return char.ConvertFromUtf32(col + 65) + "" + (row + 1);
        }

        /// <summary>
        /// When a key is pressed in the contents box
        /// If it is an enter key, input that data as the contents of the cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCellContents_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                TextBox t = (TextBox)sender;
                string contents = t.Text.ToString();
                spreadsheetPanel1.GetSelection(out int col, out int row);
                RequestType r = new RequestType("editCell", spreadsheetPanel1.ConvertCellName(col, row), contents);
                string request = JsonConvert.SerializeObject(r) + "\n";
                NC.SendData(request);
                t.Clear(); // clear the data from the textbox
                //spreadsheetPanel1.SetContents(col, row, contents);
                //spreadsheetPanel1.GetValue(col, row, out string val);
                //textBoxCellValue.Text = val;
                //textBoxCellContents.Text = spreadsheetPanel1.GetContents(col, row);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Opens up a new dialogue for spreadsheet connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.DemoApplicationContext.getAppContext().RunForm(new Form2());
        }

        /// <summary>
        /// Displays an message box if there have been any unsaved changed made to the spreadsheet.
        /// Returns true if needs cancel an exit, false otherwise.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool CheckChanged(object sender, EventArgs e)
        {
            if (spreadsheetPanel1.Changed())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes the current spreadsheet window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
 
        /// <summary>
        /// method that shows a dialogue box with information of how to operate a spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            string helpText = "This is a basic spreadsheet. To edit a cell, click on it with the mouse. " +
                "you can then edit the contents of the cell by typing in the contents and hitting enter. " +
                "The value of the cell will be shown as well as the contents of that cell." +
                " Hit the darkmode button to swap to dark mode! ";

            MessageBox.Show(helpText, "Help");
        }

        private void toolStripNightModeButton_Click(object sender, EventArgs e)
        {
            NightMode();
        }

        /// <summary>
        /// Enables/Disables Night Mode for this form.
        /// </summary>
        private void NightMode()
        {
            spreadsheetPanel1.NightMode(toolStripNightModeButton.Checked);

            if (toolStripNightModeButton.Checked)
            {
                labelCellContents.ForeColor = Color.Black;
                labelCellName.ForeColor = Color.Black;
                labelCellValue.ForeColor = Color.Black;
                this.BackColor = Color.LightGray;
                toolStripNightModeButton.Checked = false;
            }
            else
            {
                labelCellContents.ForeColor = Color.LightGray;
                labelCellName.ForeColor = Color.LightGray;
                labelCellValue.ForeColor = Color.LightGray;
                this.BackColor = ColorTranslator.FromHtml("#303030");
                toolStripNightModeButton.Checked = true;
            }
        }

        /// <summary>
        /// Sends an undo request to the server, after undo button is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UndoButton_Click(object sender, EventArgs e)
        {
            // Sending undo request to server via JSon
            UndoRequest r = new UndoRequest("undo");
            string request = JsonConvert.SerializeObject(r) + "\n";
            NC.SendData(request);
        }

        /// <summary>
        /// Sends an revert request to the server, after revert button is clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RevertButton_Click(object sender, EventArgs e)
        {
            // Sending revert request to server via JSon
            spreadsheetPanel1.GetSelection(out int col, out int row);
            RevertRequest r = new RevertRequest("revertCell", spreadsheetPanel1.ConvertCellName(col, row));
            string request = JsonConvert.SerializeObject(r) + "\n";
            NC.SendData(request);
        }
    }
}

