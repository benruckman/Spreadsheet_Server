// A server that hosts a spreadsheet
// Authors: Jackson McKay, John Richard
// Date:    4/30/21

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <errno.h>
#include <unistd.h> 
#include <arpa/inet.h> 
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/time.h> 
#include <string>
#include <thread>
#include <string.h>
#include <vector>
#include <unordered_map>
#include <iostream>
#include <sstream>
#include <dirent.h>
#include <sys/types.h>
#include <signal.h>
#include "spreadsheet.h"
#include "user.h"

// Definitions of constants needed to run
#define TRUE 1
#define PORT 1100 
#define BUFFER_SIZE 1024

// Method forwarding
void* handle_connection(void* sd);
void* update_spreadsheets(void* sd);
int error_exit(std::string err);
void shut_down(int sigint);

// Locks to ensure critical sections of code remain intact
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
pthread_mutex_t mutexsheets = PTHREAD_MUTEX_INITIALIZER;

// Max numbe of clients our server can handle
const int MAX_CLIENTS = 200;

// Array of all connected clients
user clients[MAX_CLIENTS];

// The set of spreadsheets we currently have open
std::unordered_map<string, spreadsheet*> spreads;

// the id of the thread that processes and sends updates, stored so we can kill it upon program termination
pthread_t updatethread;

/* The main entry point of our spreadsheet server, continually reads network traffic on port 1100
 *		And handles it accordingly
 * 
 * Returns: an integer representing the exit status of the program
 * 
 * Parameters: Commmand line arguments, none of which are used. 
 */
int main(int argc, char* argv[])
{
	// Register shut down signal to shut_down(), so we can clean up before the application closes
	signal(SIGINT, shut_down);
	// The master socket, where data flows into
	int master_socket; 	
	// The set of all sockets that have data waiting to be read		
	fd_set readfds; 			

	// Get the files of all the spreadsheets we can read
	struct dirent* files;
	DIR* directory = opendir("/spreadsheet_data/");

	// Try to open spreadsheet files inside directory
	string full_name = "";
	if (directory != NULL)
	{
		while ((files = readdir(directory)) != NULL)
		{
			full_name = files->d_name;
			size_t last_index = full_name.find_last_of(".");
			string raw_name = full_name.substr(0, last_index);
			string extension = full_name.substr(raw_name.length());
			if (extension == ".sprd")
			{
				// Create a new spreadsheet from file, and add it to our set
				spreadsheet* new_sheet = new spreadsheet(raw_name);
				spreads[raw_name] = new_sheet;
			}
		}
	}
	closedir(directory);

	// Initialize all spots in clients[] to have empty users
	for (int i = 0; i < MAX_CLIENTS; i++)
	{
		user u;
		clients[i] = u;
	}

	// Create a master socket (i.e. the listener)
	if ((master_socket = socket(AF_INET, SOCK_STREAM, 0)) == 0)
		return error_exit("Error creating socket");

	int option = 1;
	// Set master socket to allow multiple connections 
	if (setsockopt(master_socket, SOL_SOCKET, SO_REUSEADDR, (char*)&option, sizeof(option)) < 0)
		return error_exit("Error setting socket options");

	// Our address for our socket to bind to 
	struct sockaddr_in address;
	address.sin_family = AF_INET;
	address.sin_addr.s_addr = INADDR_ANY;
	address.sin_port = htons(PORT);

	// Bind the socket to localhost port 1100
	if (bind(master_socket, (struct sockaddr*)&address, sizeof(address)) < 0)
		return error_exit("Error binding socket");

	// Try to specify maximum of 5 pending connections for the master socket
	if (listen(master_socket, 5) < 0)
		return error_exit("Error setting up listener on master socket");

	std::cout << "Waiting for connections..." << std::endl;

	// Create and launch the thread that will process updates 
	pthread_create(&updatethread, NULL, update_spreadsheets, NULL);
	pthread_detach(updatethread);

	// Start our loop that will accept new connections, and read data from those connections
	while (TRUE)
	{
		// Clear the socket set of current reads
		FD_ZERO(&readfds);

		// Add master socket to set, so we know of any pending connections
		FD_SET(master_socket, &readfds);

		// Create a timeout for select, so it doesn't block if there is nothing to be read
		timeval timeout;
		timeout.tv_sec = 0;
		timeout.tv_usec = 50;

		// Keep track of the number of sockets we have
		int max_sd = master_socket;

		// Add additional sockets to the set, if they are valid connections
		for (int i = 0; i < MAX_CLIENTS - 1; i++)
		{
			pthread_mutex_lock(&mutex);
			int	sd = clients[i].get_socket();
			pthread_mutex_unlock(&mutex);
			if (sd > 0)
				FD_SET(sd, &readfds);
			if (sd > max_sd)
				max_sd = sd;
		}

		// Wait and see if there is any activity on our reading sockets
		int activity = select(max_sd + 1, &readfds, NULL, NULL, &timeout);
		if ((activity < 0) && (errno != EINTR))
			std::cout << "Error in select();" << std::endl;
		else if (activity == 0)
			continue;
		else
		{
			// If something happened on the master socket, it is a new connection
			if (FD_ISSET(master_socket, &readfds))
			{
				// Accept the incoming connection
				int addrlen = sizeof(address);
				int new_socket;
				if ((new_socket = accept(master_socket, (struct sockaddr*)&address, (socklen_t*)&addrlen)) < 0)
					std::cout << "Error accepting new connection" << std::endl;
				else
				{
					// Create a new thread to handle the handshake, and launch
					pthread_t pid;
					if (pthread_create(&pid, NULL, handle_connection, (void*)&new_socket) != 0)
						std::cout << "Handshake Failure" << std::endl;
					pthread_detach(pid);
				}
			}

			// Else its some IO operation on some other socket, like a read 
			for (int i = 0; i < MAX_CLIENTS - 1; i++)
			{
				// Get a client we will check
				pthread_mutex_lock(&mutex);
				int sd = clients[i].get_socket();
				user* cli = &clients[i];
				pthread_mutex_unlock(&mutex);

				// If this client has data that needs to be read
				if (FD_ISSET(sd, &readfds))
				{
					// Check if it was for closing 
					int valread;
					if ((valread = read(sd, cli->get_buffer(), 1024)) <= 0)
					{
						// A client disconnected, print info
						std::cout << "User " << cli->get_username() << " disconnected from spreadsheet " << cli->get_ssname() << std::endl;

						// Remove the user from the spreadsheet they were on, and send a disconnect message to other users
						pthread_mutex_lock(&mutexsheets);
						auto it = spreads.find(cli->get_ssname());
						if (it == spreads.end())
							std::cout << "Error: removing client from nonexistant spreadsheet" << std::endl;
						else
						{
							spreadsheet* s = it->second;
							s->remove_user(cli->get_id());
							s->send_disconnect(cli->get_id());
						}
						pthread_mutex_unlock(&mutexsheets);

						// close the socket they were on, and remove them from the client list
						close(sd);
						pthread_mutex_lock(&mutex);
						user u;
						clients[i] = u;
						pthread_mutex_unlock(&mutex);
					}

					// Else it is a message that has come in that needs to be processed
					else
					{
						// Get the message, and parse it
						std::string message = cli->get_buffer();
						cli->clear_buffer();
						pthread_mutex_lock(&mutexsheets);
						auto it = spreads.find(cli->get_ssname());
						spreadsheet* s = it->second;
						cli->add_data(message);
						std::string* data = cli->get_data();
						std::string delimiter = "\n";
						size_t pos = 0;
						std::string token;
						while ((pos = data->find(delimiter)) != std::string::npos) 
						{
    						token = data->substr(0, pos);
    						cli->remove_data(0, data->find(delimiter) + delimiter.length());
							if(token.size() > 1)
							{
								// If we have a valid message, add it to the queue of things to be processed
								s->add_message(token, cli->get_id());
							}
						}
						cli->add_data(*data);
						pthread_mutex_unlock(&mutexsheets);
					}
				}
			}
		}
	}
	return 0;
}

/* Handles the handshake process when a new client tries to connect
 * 		
 * Returns: Nothing
 * 
 * Parameters: The socket on which the client is connecting from 
 */
void* handle_connection(void* sd)
{
	int newfd = *(int*)sd;
	char buf[BUFFER_SIZE];
	int nbytes;
	bzero(buf, BUFFER_SIZE);

	// Read the first thing from the client, should be their username
	nbytes = read(newfd, buf, BUFFER_SIZE);
	if (nbytes < 0)
	{
		close(newfd);
		pthread_exit(0);
	}
	if (nbytes == 0)
	{
		close(newfd);
		pthread_exit(0);
	}
	std::string name = std::string(buf);
	name.erase(name.end() - 1);
	bzero(buf, BUFFER_SIZE);

	// Send the client the list of possible spreadsheets to edit
	pthread_mutex_lock(&mutexsheets);
	if (spreads.empty())
	{
		std::string s = "\n\n";
		int n = s.length();
		char message[n + 1];
		strcpy(message, s.c_str());
		send(newfd, message, strlen(message), 0);
	}
	else
	{
		std::string list;
		for (auto it : spreads)
		{
			list.append(it.first);
			list.append("\n");
		}
		list.append("\n");
		int n = list.length();
		char files[n + 1];
		strcpy(files, list.c_str());
		send(newfd, files, strlen(files), 0);
	}
	pthread_mutex_unlock(&mutexsheets);


	// Read the name of the spreadsheet the client wishes to edit
	nbytes = read(newfd, buf, BUFFER_SIZE);
	if (nbytes < 0)
	{
		close(newfd);
		pthread_exit(0);
	}
	if (nbytes == 0)
	{
		close(newfd);
		pthread_exit(0);
	}
	std::string ssname = std::string(buf);
	ssname.erase(ssname.end() - 1);

	// Add a new user to map of sockets, and spreadsheet to list of spreadsheets
	pthread_mutex_lock(&mutex);
	for (int i = 0; i < MAX_CLIENTS - 1; i++)
	{
		if (clients[i].get_id() < 0)
		{
			// We have a spot for a new user
			user* u = new user(i, newfd, name, ssname);
			clients[i] = *u;

			std::cout << "Created new user " << name << std::endl;

			pthread_mutex_lock(&mutexsheets);
			// If we do not already have the spreadsheet, make a new one
			auto it = spreads.find(ssname);
			if (it == spreads.end())
			{
				spreadsheet* s = new spreadsheet(ssname);
				s->add_user(u);
				spreads[ssname] = s;
		
				std::cout << "Added user " << name << " to new spreadsheet " << ssname << " with ID " << i << std::endl;
			}
			// Else they are opening a new one
			else
			{
				spreadsheet* s = it->second;
				std::cout << "Added user " << name << " to existing spreadsheet " << ssname << " with ID " << i << std::endl;
				// send user state of spreadsheet
				s->send_spreadsheet(u->get_socket());
				s->send_selections(u->get_socket());
				s->add_user(u);
			}
			
			// Finally, send user their unique ID
			std::stringstream ss;
			ss << u->get_id() << "\n";
			int length = ss.str().length();
			char message[length + 1];
			strcpy(message, ss.str().c_str());
			send(newfd, message, strlen(message), 0);
			pthread_mutex_unlock(&mutexsheets);
			break;
		}
	}
	pthread_mutex_unlock(&mutex);
	pthread_exit(0);
	return 0;
}

/* Continually updates the state of spreadsheets based on user requests
 * 		
 * Returns: Nothing
 * 
 * Parameters: None
 */
void* update_spreadsheets(void* sd)
{
	while (true)
	{
		pthread_mutex_lock(&mutexsheets);
		if (!spreads.empty())
		{
			// For every spreadsheet in the list, process pending  messages
			for (auto it : spreads)
			{
				string key = it.first;
				auto value = spreads.find(key);
				spreadsheet* s = value->second;
				s->process_messages();
			}
		}
		pthread_mutex_unlock(&mutexsheets);
	}
}

/* Exits the program if an error is encountered
 * 		
 * Returns: The error status 
 * 
 * Parameters: The error message
 */
int error_exit(std::string err)
{
	std::cout << err << std::endl;
	shut_down(SIGINT);
	return -1;
}

/* Cleanly shuts down our server, by saving the state of the spreadsheets
 * 		
 * Returns: Nothing
 * 
 * Parameters: The exit signal
 */
void shut_down(int sigint)
{
	std::cout << "Cleaning up" << std::endl;

	// Send a server closing message to all of our connected clients
	pthread_mutex_lock(&mutex);
	for (int i = 0; i < MAX_CLIENTS - 1; i++)
	{
		if (clients[i].get_id() != -1)
		{
			std::cout << "Sending disconnect message to "<< clients[i].get_username() << std::endl;
			spreadsheet ss("");
			string s = ss.serialize_server_shutdown("serverError", "Server is closing");
			int n = s.length();
			char message[n + 1];
			strcpy(message, s.c_str());
			send(clients[i].get_socket(), message, strlen(message), 0);
		}
	}
	pthread_mutex_unlock(&mutex);

	// Save all of the spreadsheets
	pthread_mutex_lock(&mutexsheets);
	for (auto it : spreads)
	{
		std::cout << "Saving spreadsheet "<< it.first << std::endl;
		string key = it.first;
		auto value = spreads.find(key);
		spreadsheet* s = value->second;
		s->save();
	}
	pthread_mutex_unlock(&mutexsheets);

	// Kill the processing thread, and exit the program
	pthread_cancel(updatethread); 
	std::cout << "Exiting" << std::endl;
	exit(0);
}