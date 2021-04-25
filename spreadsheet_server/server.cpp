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
#include<unordered_map>
#include "spreadsheet.h"
#include "user.h"

#define TRUE 1
#define PORT 1100
#define BUFFER_SIZE 1024


// used to lock critical sections of code, mainly clients[].
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

// used to lock critical sections of code, spreads
pthread_mutex_t mutexsheets = PTHREAD_MUTEX_INITIALIZER;

// handles the handshake
void* handle_connection(void* sd);

// updates spreadsheets constantly
void* update_spreadsheets(void* sd);

int error_exit(std::string err);

const int MAX_CLIENTS = 200;
// array of all client clients, let us start wit
user clients[MAX_CLIENTS];

std::unordered_map<string, spreadsheet> spreads;

// the main entry point of our application
int main(int argc, char* argv[])
{
	int master_socket; 			// our master socket, where data flows into
	int	max_clients = 30; 		// the maximum amount of clients our server can handle
	fd_set readfds; 			// the set of all reading sockets


	// a testing message
	char* message = "{\"messageType\" : \"cellUpdated\", \"cellName\" : \"a1\", \"contents\" : \"=1 + 3\"}\n";

	// initialize all client_socket[] with empty users
	for (int i = 0; i < MAX_CLIENTS; i++)
	{
		user u;
		clients[i] = u;
	}

	// create a master socket (i.e. the listener)
	if ((master_socket = socket(AF_INET, SOCK_STREAM, 0)) == 0)
		return error_exit("Error creating socket");
	
	int option = 1;	
	// set master socket to allow multiple connections 
	if (setsockopt(master_socket, SOL_SOCKET, SO_REUSEADDR, (char*)&option, sizeof(option)) < 0)
		return error_exit("Error setting socket options");

	// our address for our socket to bind to 
	struct sockaddr_in address; 
	address.sin_family = AF_INET;
	address.sin_addr.s_addr = INADDR_ANY;
	address.sin_port = htons(PORT);

	// bind the socket to localhost port 1100
	if (bind(master_socket, (struct sockaddr*)&address, sizeof(address)) < 0)
		return error_exit("Error binding socket");

	// try to specify maximum of 5 pending connections for the master socket
	if (listen(master_socket, 5) < 0)
		return error_exit("Error setting up listener on master socket");

	std::cout << "Waiting for connections..." << std::endl;

	// thread to constantly update spreadsheets
	pthread_t updatethread;
	pthread_create(&updatethread, NULL, update_spreadsheets, NULL);
	pthread_detach(updatethread);
	// REMEMEBER TO KILL ME WHEN MAIN ENDS

	while (TRUE)
	{
		// clear the socket set of current reads
		FD_ZERO(&readfds);

		// add master socket to set, so we know of any pending connections
		FD_SET(master_socket, &readfds);

		// create a timeout for select
		timeval timeout;
		timeout.tv_sec = 0;
		timeout.tv_usec = 50;

		// update the size accordingly
		int max_sd = master_socket;

		// add additional sockets to set
		for (int i = 0 ; i < MAX_CLIENTS; i++)
		{
			pthread_mutex_lock(&mutex);
			int	sd = clients[i].get_socket();
			pthread_mutex_unlock(&mutex);
			if(sd > 0)
				FD_SET( sd , &readfds);
			if(sd > max_sd)
				max_sd = sd;
		}

		// wait and see if there is any activity on our reading sockets
		int activity = select(max_sd + 1, &readfds, NULL, NULL, &timeout);
		if ((activity < 0) && (errno != EINTR))
			std::cout << "Error in select();" << std::endl;
		else if (activity == 0)
			continue;
		else
		{

			// if something happened on the master socket, it is a new connection
			if (FD_ISSET(master_socket, &readfds))
			{
				// accept the incoming connection
				int addrlen = sizeof(address);
				int new_socket;
				if ((new_socket = accept(master_socket, (struct sockaddr*)&address, (socklen_t*)&addrlen)) < 0)
					std::cout << "Error accepting new connection" << std::endl;
				else
				{
					// create a new thread to handle the handshake, and launch
					pthread_t pid;
					if (pthread_create(&pid, NULL, handle_connection, (void*)&new_socket) != 0)
						std::cout << "Handshake Failure" << std::endl;
					// detach the thread
					pthread_detach(pid);
				}
			}

			// else its some IO operation on some other socket
			for (int i = 0; i < MAX_CLIENTS; i++)
			{
				// 
				pthread_mutex_lock(&mutex);
				int sd = clients[i].get_socket();
				user* cli = &clients[i];
				pthread_mutex_unlock(&mutex);

				if (FD_ISSET(sd, &readfds))
				{
					// Check if it was for closing 
					int valread;
					if ((valread = read(sd, cli->get_buffer(), 1024)) <= 0)
					{
						// a client disconnected, print info
						std::cout << "User " << cli->get_username() << " disconnected from spreadsheet " << cli->get_ssname() << std::endl;
						
						// remove the user from the spreadsheet they were on
						pthread_mutex_lock(&mutexsheets);
						auto it = spreads.find(cli->get_ssname());
  						if (it == spreads.end())
							std::cout << "Error: removing client from nonexistant spreadsheet" << std::endl;
  						else
						{
							spreadsheet* s = &it->second;
							s->remove_user(cli->get_id());
						}
						pthread_mutex_unlock(&mutexsheets);
						
						// close the socket they were on, and remove them from the client list
						close(sd);
						pthread_mutex_lock(&mutex);
						user u;
						clients[i] = u;
						pthread_mutex_unlock(&mutex);
						
					}
					// a message came in, print it
					else
					{
						std::string message = cli->get_buffer();
						cli->clear_buffer();
						pthread_mutex_lock(&mutexsheets);
						auto it = spreads.find(cli->get_ssname());
						spreadsheet* s = &it->second;
						std::cout << "ADDED MESSAGE TO QUEUE" << std::endl;
						s->add_message(message);
						pthread_mutex_unlock(&mutexsheets);

						// updating and sending info out in new thread, not sure if it is the move -Jackson
						pthread_t updatethread;
						pthread_create(&updatethread, NULL, update_spreadsheets, NULL);
						pthread_detach(updatethread);


					}
				}
			}
		}
	}
	return 0;
}

// handles the handshake process 
void* handle_connection(void* sd)
{
	int newfd = *(int*)sd;
	char buf[BUFFER_SIZE];
	int nbytes;

	bzero(buf, BUFFER_SIZE);

	// read the first thing from the client, the username
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
	name.erase(name.end()-1);

	// clear our buffer out
	bzero(buf, BUFFER_SIZE);

	// send the client the list of possible spreadsheets
	pthread_mutex_lock(&mutexsheets);
	if(spreads.empty())
	{
		char* message = "\n\n";
		send(newfd, message, strlen(message), 0);
	}	
	else
	{
		std::string list;
		for(auto it : spreads)
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
	

	// read the name of the spreadsheet the client wishes to edit
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
	ssname.erase(ssname.end()-1);

	// add a new user to map of sockets, and to list of spreadsheets
	pthread_mutex_lock(&mutex);
	for(int i = 0; i < MAX_CLIENTS; i++)
	{
		if(clients[i].get_id() < 0)
		{
			// we have a spot for a new user
			user* u = new user(i, newfd, name, ssname);
			clients[i] = *u;
			//pthread_mutex_lock(&mutexsheets);
			std::cout <<"Created new user " << name << std::endl;

			pthread_mutex_lock(&mutexsheets);
			// if we do not already have the spreadsheet, make a new one
			auto it = spreads.find(ssname);
  			if (it == spreads.end())
			{
				spreadsheet s(ssname);
				s.add_user(u);
				spreads[ssname] = s;
				std::cout <<"Added user " << name << " to new spreadsheet " << ssname << " with ID " << i << std::endl;
			}
  			else
			{
				spreadsheet* s = &it->second;
				s->add_user(u);
				std::cout <<"Added user " << name << " to existing spreadsheet " << ssname << " with ID " << i << std::endl;
			}
			pthread_mutex_unlock(&mutexsheets);
			break;
		}
	}
	pthread_mutex_unlock(&mutex);
	pthread_exit(0);
	return 0;

}

// updates spreadsheets
void* update_spreadsheets(void* sd)
{
		pthread_mutex_lock(&mutexsheets);	
		if(!spreads.empty())
		{
			spreadsheet* s;
			for(auto it : spreads)
			{
				it.second.process_messages();
			}
		}
		pthread_mutex_unlock(&mutexsheets);	
}

// exits the program printing an error message
int error_exit(std::string err)
{
	std::cout << err << std::endl;
	return -1;
}







