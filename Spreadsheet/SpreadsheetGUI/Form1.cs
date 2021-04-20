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
        SS.NetworkControl NC;

        /// <summary>
        /// Creates a new window displaying an empty spreadsheet
        /// </summary>
        public Form1(SS.NetworkControl NetC, string filename)
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

        private void ServerUpdate(messageType message)
        {
            if (message.type.Equals("cellUpdated"))
            {
                int col = spreadsheetPanel1.GetCellNameCol(message.cellName);
                int row = spreadsheetPanel1.GetCellNameRow(message.cellName);
                spreadsheetPanel1.SetContents(col, row, message.cellContents);
                spreadsheetPanel1.GetValue(col, row, out string val);
            }

            // TODO:IF SEL SELECTION REQUEST

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

            //SEND SHIT
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
                requestType r = new requestType("editCell", spreadsheetPanel1.ConvertCellName(col, row), contents);
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
        /// Method that reads a spreadsheet file, and opens it up,
        /// and displays it in the current working spreadsheet window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check to see if the current spreadsheet has been edited. 
            CheckChanged(sender, e);
            try
            {
                using (OpenFileDialog fileDialog = new OpenFileDialog())
                {
                    fileDialog.Filter = "Spreadsheet|*.sprd|All Files| *.*";
                    fileDialog.DefaultExt = ".sprd";
                    fileDialog.Title = "Open a file";
                    fileDialog.ShowDialog();

                    if (fileDialog.FileName != "")
                    {
                        // Set Title to filename
                        Text = fileDialog.SafeFileName;

                        // Clear previous data
                        spreadsheetPanel1.Clear();
                        textBoxCellValue.Text = "";
                        textBoxCellContents.Text = "";

                        // Import new data and set focus on top leftmost cell.
                        spreadsheetPanel1.Import(fileDialog.FileName);
                        spreadsheetPanel1.SetSelection(0, 0);
                    }
                }
            }
            catch (SpreadsheetReadWriteException)
            {
                DialogResult r = MessageBox.Show("FileReadingError: Cannot Open the file as a spreadsheet because it is incompatable.",
              "Invalid File",
              MessageBoxButtons.OK);
            }
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
                DialogResult r = MessageBox.Show("You have unsaved data that will be lost. Would you like to save?",
              "Unsaved Document",
              MessageBoxButtons.YesNoCancel);

                switch (r)
                {
                    case DialogResult.Yes:
                        saveToolStripMenuItem_Click(sender, e);
                        return true;
                    case DialogResult.No:
                        return false;
                    case DialogResult.Cancel:
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new spreadsheet in a new window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            Program.DemoApplicationContext.getAppContext().RunForm(new Form1(NC, this.Text));
        }

        /// <summary>
        /// Saves the current spreadsheet to file the user specifies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the spreadsheet
            using (SaveFileDialog s = new SaveFileDialog())
            {
                s.Filter = "Spreadsheet|*.sprd";
                s.Title = "Save a spreadsheet";
                s.DefaultExt = ".sprd";
                s.AddExtension = true;
                s.ShowDialog();

                // If the file name isn't an empty string, open it for saving.
                if (s.FileName != "")
                {
                    spreadsheetPanel1.Save(s.FileName);
                    Text = Path.GetFileName(s.FileName);
                }
            }
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
        /// This method is called anytime the form closes
        /// checks to see if there is any unsaved progress,
        /// and prompts user accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            

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
    }
}

