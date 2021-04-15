using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            DisplaySpreadsheetNames.Text = "Hello" + Environment.NewLine + "Darkness\nMy\nOld\nFriend"; //Enviornment.NewLine is how to get a newline in a text box. 
        }

        private void OpenSpreadsheetButtonPressed(object sender, EventArgs e)
        {
            String spreadsheetName = InputSpreadsheetName.Text;
            //TODO: Send this name to server
        }
    }
}
