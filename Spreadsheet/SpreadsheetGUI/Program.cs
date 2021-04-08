using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;

namespace SpreadsheetGUI
{
    static class Program
    {
        /// <summary>
        /// Keeps track of how many top-level forms are running
        /// </summary> 
        public class DemoApplicationContext : ApplicationContext
        {
            // Number of open forms
            private int formCount = 0;

            // Singleton ApplicationContext
            private static DemoApplicationContext appContext;

            /// <summary>
            /// Private constructor for singleton pattern
            /// </summary>
            private DemoApplicationContext()
            {
            }

            /// <summary>
            /// Returns the one DemoApplicationContext.
            /// </summary>
            public static DemoApplicationContext getAppContext()
            {
                if (appContext == null)
                {
                    appContext = new DemoApplicationContext();
                }
                return appContext;
            }

            /// <summary>
            /// Runs the form
            /// </summary>
            public void RunForm(Form form)
            {

                // One more form is running
                formCount++;

                // When this form closes, we want to find out
                form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

                // Run the form
                form.Show();
            }

        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // some settngs we want our forms to have
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // launch our setup form
            Application.Run(new Form2());
        }
    }
}
