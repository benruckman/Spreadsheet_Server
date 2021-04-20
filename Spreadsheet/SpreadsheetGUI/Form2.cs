using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    public partial class Form2 : Form
    {
        // the interface we will use to interact with the server
        SS.NetworkControl NetC;

        /// <summary>
        /// Creates an initial setup form
        /// </summary>
        /// <param name="nc"></param>
        public Form2()
        {
            NetC = new SS.NetworkControl();
            InitializeComponent();
            NetC.Connected += Connected;
            NetC.Error += ErrorRecieved;
            NetC.Names += DisplayNames;
        }

        private void DisplayNames(string name)
        {
            this.Invoke(new MethodInvoker(
             () => DisplaySheets.Text = DisplaySheets.Text + name + Environment.NewLine));

            this.Invoke(new MethodInvoker(
             () => TextBoxSpreadsheetName.Focus()));
        }

        private void ErrorRecieved(string err)
        {
            MessageBox.Show(err);
        }

        /// <summary>
        /// Once we have connected to the server
        /// </summary>
        private void Connected()
        {
            // disable controls
            this.Invoke(new MethodInvoker(
             () => ButtonConnect.Enabled = false));
            this.Invoke(new MethodInvoker(
             () => TextBoxAddress.Enabled = false));
            this.Invoke(new MethodInvoker(
             () => TextBoxUserName.Enabled = false));

            // enable others
            this.Invoke(new MethodInvoker(
             () => ButtonSelect.Enabled = true));
            this.Invoke(new MethodInvoker(
             () => TextBoxSpreadsheetName.Enabled = true));
        }

        /// <summary>
        /// Attempts to make a connection to a server with a username, specified by the data in textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if(TextBoxAddress.Text != "" && TextBoxUserName.Text != "")
            {
                NetC.Connect(TextBoxAddress.Text, TextBoxUserName.Text);
                ButtonConnect.Enabled = false;
                TextBoxAddress.Enabled = false;
                TextBoxUserName.Enabled = false;
            }
            else
            {
                MessageBox.Show("Please enter valid address and username");
            }
        }

        private void ButtonSelect_Click(object sender, EventArgs e)
        {
            if (TextBoxSpreadsheetName.Text != "" && TextBoxUserName.Text != "")
            {
                NetC.SendData(TextBoxSpreadsheetName.Text);
                ButtonSelect.Enabled = false;
                TextBoxSpreadsheetName.Enabled = false;

                // open our spreadsheet form
                Program.DemoApplicationContext.getAppContext().RunForm(new Form1(NetC));
                this.Hide();

            }
            else
            {
                MessageBox.Show("Please enter valid spreadsheet name");
            }
        }
    }
}
