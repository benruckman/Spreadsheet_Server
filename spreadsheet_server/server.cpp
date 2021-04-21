#include "user.h"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <thread>
#include <string>
#include <string.h>
#include <map>
#include <stdio.h>

#define PORT 1100
#define BUFFER_SIZE 1024

void handle_connect(int socket_id);

int main(int argc, char const *argv[])
{
    // Initialize variables
    int listener_fd;
    int chars_read;
    int new_socket;
    int max_sd;
    int current_sd;
    int activity;
    int valread;
    int max_clients = 5; // Change when we settle on max users
    int client_socket[5]; // Change when we settle on max users
    char buffer[BUFFER_SIZE] = {0};
    struct sockaddr_in server_address;
    int address_length = sizeof(server_address);
    
    // Create socket
    listener_fd = socket(AF_INET, SOCK_STREAM, 0);

    // Check for error
    if(listener_fd < 0)
    {
        perror("Socket failed to open.");
        exit(EXIT_FAILURE);
    }
  
    // Set the options
    // setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR | SO_REUSEPORT, &opt, sizeof(opt))

    // Set up server address structure
    server_address.sin_family = AF_INET;
    server_address.sin_addr.s_addr = INADDR_ANY;
    server_address.sin_port = htons( PORT );

    // Binding the socket to the host address
    if(bind(listener_fd, (struct sockaddr *) &server_address, address_length) < 0)
    {
        perror("Binding the socket failed.");
        exit(EXIT_FAILURE);
    }

    // Begins listening for new connections
    if(listen(listener_fd, 15) < 0)
    {
        perror("Error while listening.");
        exit(EXIT_FAILURE);
    }

    std::cout << "Listening for connection..." << std::endl;


    fd_set readfds;

    // Clear an fd_set
    FD_ZERO(&readfds);  
  
    // Add a descriptor to an fd_set
    FD_SET(listener_fd, &readfds);   
  
    // Remove a descriptor from an fd_set
    FD_CLR(listener_fd, &readfds); 
  
    //If something happened on the master socket , then its an incoming connection  
    FD_ISSET(listener_fd, &readfds); 
  

    while(true)
    {
        //clear the socket set 
        FD_ZERO(&readfds);  
     
        //add master socket to set 
        FD_SET(listener_fd, &readfds);  
        max_sd = listener_fd;  
         
        //add child sockets to set
        for ( int i = 0 ; i < max_clients -1 ; i++)
        {  
            //socket descriptor
            current_sd = client_socket[i];

            //if valid socket descriptor then add to read list 
            if(current_sd > 0)  
                FD_SET( current_sd , &readfds);  
                
            //highest file descriptor number, need it for the select function 
            if(current_sd > max_sd)  
                max_sd = current_sd;  
        }  
    
        //wait for an activity on one of the sockets , timeout is NULL , 
        //so wait indefinitely 
        activity = select( max_sd + 1 , &readfds , NULL , NULL , NULL);  
   
        if ((activity < 0) && (errno!=EINTR))  
        {  
            printf("select error");  
        }  
         
        //If something happened on the master socket , 
        //then its an incoming connection 
        if (FD_ISSET(listener_fd, &readfds))  
        {  
            // Accepts a new connection
            if ((new_socket = accept(listener_fd, (struct sockaddr *) &server_address, (socklen_t *) &address_length)) < 0)
            {
                perror("Error while accepting.");
                exit(EXIT_FAILURE);
            }
    
            std::cout << "New user connecting" << std::endl;
    
            std::thread t(handle_connect, new_socket);
            t.detach();
      
            //add new socket to array of sockets 
            for (int i = 0; i < max_clients; i++)  
            {  
                //if position is empty 
                if( client_socket[i] == 0 )  
                {  
                    client_socket[i] = new_socket;  
                    printf("Adding to list of sockets as %d\n" , i);  
                   
                    break;  
                }  
            }  
        }  
        
        //else its some IO operation on some other socket
        for (int i = 0; i < max_clients; i++)  
        {  
//             // handling no data from our client
// 			if ((valread = recv(i, buffer, sizeof(buffer), 0)) <= 0)
// 			//if((nbytes = read(i, buf, 1024)) <= 0)
// 			{
// 				/* got error or connection closed by client */
// 				if (valread == 0)
// 					printf("%s: socket %d hung up\n", argv[0], i);
// 			    else
// 					perror("Error recieving data");

// 			    // close the socket, as it is dead
// 			    close(i); 

// 				// remove the dead socket from our set
// 			   	FD_CLR(i, &master);
// 			}
	        
// 			// process data from our client
// 			else
// 			{
// 				// we have data from a client!
// 				for (j = 0; j <= fdmax; j++)
// 				{
// 					// send the data to everyone
// 					if (FD_ISSET(j, &master))
// 					{
// 					    perror(buffer);

// 						// except to the listener
// 						if (j != listener)
// 						{
// 							char *file_list = "{\"messageType\" : \"cellUpdated\", \"cellName\" : \"a1\", \"contents\" : \"=1 + 3\"}\n";
// 							if (send(j, file_list, strlen(file_list), 0) == -1)
//     							perror("Error sending data");
// 						}
// 					}
// 				}
// 			}
        }  
    } 
  
    return 0;
}

/*
 * Method is used to complete the handshake on its own thread.
 * 
 */
void handle_connect(int socket_id)
{
  user new_user(BUFFER_SIZE);
  new_user.set_id(socket_id);
 
  int num_char = read(new_user.get_id(), new_user.get_buffer(), BUFFER_SIZE);
  
  std::string username = "";
  char current =  new_user.get_buffer()[0];
  int i = 0;

  while(current != '\n')
  {
    username += current;
    i++;
    current = new_user.get_buffer()[i];
  }
  
  bzero(new_user.get_buffer(), BUFFER_SIZE);
  
  std::cout << "Username: " << username << std::endl;
  std::cout << "Socket ID: " << socket_id << std::endl;

  // Set the username
  new_user.set_username(username);

  // Send a list of file names
  char *file_list = "Spreadsheet1\nDonnieDarko\nVinylRecordsAreCool\n\n";
  
  std::cout << "Sending: " << "Spreadsheet1\nDonnieDarko\nVinylRecordsAreCool\n\n" << std::endl;
  
  send(new_user.get_id(), file_list, strlen(file_list), 0);
  
  // Read response
  num_char = read(new_user.get_id(), new_user.get_buffer(), 1024);

  // Read the second message from client. Message should be the file name
  std::string file_name = "";
  current = new_user.get_buffer()[0];
  i = 0;
  
  while(current != '\n')
  {
    file_name += current;
    i++;
    current = new_user.get_buffer()[i];
  }
  
  bzero(new_user.get_buffer(), BUFFER_SIZE);
  
  std::cout << "File name: " << file_name << std::endl;
}