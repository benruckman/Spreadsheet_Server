using System;
using System.Text.RegularExpressions;
using NetworkUtil;

namespace SS
{
    public class NetworkControl
    {
        // Events that the view/model can subscribe to
        public delegate void ErrorHandler(string err);
        public delegate void UpdateFromServer(string update);
        public delegate void ConnectedToServer();
        public delegate void NamesFromServer(string name);
        public event ErrorHandler Error;
        public event UpdateFromServer Update;
        public event ConnectedToServer Connected;
        public event NamesFromServer Names;


        // The spreadsheet needs to send information about itself to the server
        // That information is stored in these variables
        private string clientName;
        private int clientID;

        // State representing the connection to the server
        public SocketState theServer;

        // Creates a control object 
        public NetworkControl()
        {
            theServer = null;
            clientID = -1;
            clientName = null;
        }


        /// <summary>
        /// Atttemps to connect to a server at address
        /// </summary>
        /// <param name="address">The address of the server we are connecting to </param>
        /// <param name="name">The name of the client connecting</param>
        public void Connect(string address, string name)
        {
            // save the name of the client connecting
            clientName = name;

            // Attempt to connect to the server
            Networking.ConnectToServer(OnConnect, address, 11000);
        }

        /// <summary>
        /// Callback for the connect method
        /// </summary>
        /// <param name="state"></param>
        private void OnConnect(SocketState state)
        {
            // check to see if our connection is was succesful
            if (state.ErrorOccured)
            {
                // inform the view if we have an error
                Error("Error connecting to server " + state.ErrorMessage);
                state.OnNetworkAction = (SocketState) => { };
                return;
            }
            else
            {
                // set our server to this new connection
                theServer = state;

                // tell the server who we are
                SendUserName();

                // inform the view via an event
                Connected();

                // assign the network action event to our recieve data event
                state.OnNetworkAction = ReceiveSpreadsheetNames;

                // start receiving spreadsheetnames
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// Callback for the OnConnect method
        /// Recieves initial data about the names of the initial spreadsheets
        /// </summary>
        /// <param name="state"></param>
        private void ReceiveSpreadsheetNames(SocketState state)
        {
            // check to see if we still have a connection
            if (state.ErrorOccured)
            {
                // inform the view if we have an error
                Error("Lost connection to server " + state.ErrorMessage);
                return;
            }

            // startup: show the client the different spreadsheets

            // get the Json information
            string jsonInfo = state.GetData();

            // split it into actual messages
            string[] parts = Regex.Split(jsonInfo, @"(?<=[\n])");

            //string test = "Broadcast\nBlouse\nBeachhouse\n\n";

            //string[] tester = Regex.Split(test, @"(?<=[\n])");

            foreach (string p in parts)
            {
                // ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;

                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                // if we have recieved all names
                if (p.Equals("\n"))
                {
                    state.OnNetworkAction = ReceiveUpdates;
                    state.RemoveData(0, p.Length);
                    continue;
                }
                    

                // remove the data we just processed from the state's buffer
                state.RemoveData(0, p.Length);

                Names(p);

            }

            //starting another recieve data event loop
            // if we have recieved all of the names, change the network callback
            //state.OnNetworkAction = ReceiveSpreadsheetData;
            Networking.GetData(state);
        }

      

        /// <summary>
        /// Recieves data from the server
        /// </summary>
        /// <param name="state">The SocketState containing our server</param>
        private void ReceiveUpdates(SocketState state)
        {
            // check to see if we still have a connection
            if (state.ErrorOccured)
            {
                // inform the view if we have an error
                Error("Lost connection to server" + state.ErrorMessage);
                return;
            }

            // process the data that we already have
            ProcessUpdates(state);

            // continue event loop
            Networking.GetData(state);
        }

        /// <summary>
        /// Parses the Json information recieved from the server
        /// </summary>
        /// <param name="state"></param>
        private void ProcessUpdates(SocketState state)
        {
            // get the Json information
            string jsonInfo = state.GetData();

            // split it into actual messages
            string[] parts = Regex.Split(jsonInfo, @"(?<=[\n])");

            // loop until we have processed all received data.
            foreach (string p in parts)
            {
                // ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != '\n')
                    break;

                //DEBUGGING
                Update(p);

                // remove the data we just processed from the state's buffer
                state.RemoveData(0, p.Length);
            }

        }

        /// <summary>
        /// Sends data s to the server, followed by a newline character
        /// </summary>
        public void SendData(string s)
        {
            if (theServer != null)
                Networking.Send(theServer.TheSocket, s + "\n");
        }

        /// <summary>
        /// sends the clients name to the server
        /// </summary>
        /// <param name="message"></param>
        public void SendUserName()
        {
            if (theServer != null)
                Networking.Send(theServer.TheSocket, this.clientName + "\n");
        }

    }
}
