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
        NetworkControl NetC;

        /// <summary>
        /// Creates an initial setup form
        /// </summary>
        /// <param name="nc"></param>
        public Form2()
        {
            NetC = new NetworkControl();
            InitializeComponent();
            NetC.Connected += Connected;
            NetC.Error += ErrorRecieved;
            NetC.Update += UpdateRecieved;
        }

        private void ErrorRecieved(string err)
        {

        }

        private void UpdateRecieved()
        {

        }

        /// <summary>
        /// Once we have connected to the server
        /// </summary>
        private void Connected()
        {
            // now we will recieve the list of spreadsheet names
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
            }
            else
            {
                MessageBox.Show("Please enter valid address and username");
            }
        }
    }
}
