#include <iostream>
#include <stdio.h>
#include <string.h> 
#include <stdlib.h>
#include <errno.h>
#include <unistd.h> 
#include <arpa/inet.h> 
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/time.h> 
#include <string>
#include <queue>

	
#define TRUE 1
#define FALSE 0
#define PORT 1100
#define BUFFER_SIZE 1024


// we might need to lock up some things
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

// handles the handshake
void* handle_connection(void* sd);

// list of all of our currently connected clients
int client_socket[30];

// the main entry point of our application
int main(int argc , char *argv[])
{
	int opt = TRUE;
	int master_socket;
	int addrlen ;
	int new_socket;
	//int client_socket[30];
	int	max_clients = 30;
	int activity;
	int i; 
	int valread;
	int sd;
	int max_sd;
	struct sockaddr_in address;
	
		
	char buffer[1024]; 
		
	// set of socket descriptors
	fd_set readfds;
		
	// a testing message
	char *message = "{\"messageType\" : \"cellUpdated\", \"cellName\" : \"a1\", \"contents\" : \"=1 + 3\"}\n";
	
	// initialise all client_socket[] to 0 so not checked
	for (i = 0; i < max_clients; i++)
	{
		client_socket[i] = 0;
	}
		
	// create a master socket (i.e. the listener)
	if( (master_socket = socket(AF_INET , SOCK_STREAM , 0)) == 0)
	{
		perror("socket failed");
		exit(EXIT_FAILURE);
	}
	
	// set master socket to allow multiple connections 
	if( setsockopt(master_socket, SOL_SOCKET, SO_REUSEADDR, (char *)&opt, sizeof(opt)) < 0 )
	{
		perror("setsockopt");
		exit(EXIT_FAILURE);
	}
	
	//type of socket created
	address.sin_family = AF_INET;
	address.sin_addr.s_addr = INADDR_ANY;
	address.sin_port = htons( PORT );
		
	// bind the socket to localhost port 1100
	if (bind(master_socket, (struct sockaddr *)&address, sizeof(address))<0)
	{
		perror("bind failed");
		exit(EXIT_FAILURE);
	}

	printf("Listener on port %d \n", PORT);
		
	// try to specify maximum of 5 pending connections for the master socket
	if (listen(master_socket, 5) < 0)
	{
		perror("listen");
		exit(EXIT_FAILURE);
	}
		
	// accept the incoming connection
	addrlen = sizeof(address);
	puts("Waiting for connections ...");
		
	while(TRUE)
	{
		// clear the socket set of current reads
		FD_ZERO(&readfds);

		// add master socket to set, so we know of any pending connections
		FD_SET(master_socket, &readfds);
		timeval timeout;	
		timeout.tv_sec = 0;
    	timeout.tv_usec = 50;
		
		// update the size accordingly
		max_sd = master_socket;
			
		// add additional sockets to set
		for ( i = 0 ; i < max_clients ; i++)
		{			
			pthread_mutex_lock(&mutex);
			sd = client_socket[i];
			pthread_mutex_unlock(&mutex);
			if(sd > 0)
			{
				FD_SET( sd , &readfds);
			}
			// highest file descriptor number, need it for the select function
			if(sd > max_sd)
				max_sd = sd;
		}

		// wait for an activity on one of the sockets , timeout is NULL ,
		// so wait indefinitely
		activity = select( max_sd + 1 , &readfds , NULL , NULL , &timeout);
		
		if ((activity < 0) && (errno!=EINTR))
		{
			printf("select error");
		}
		else if(activity == 0)
		{	
			continue;
		}
		else
		{	
		// If something happened on the master socket,
		// then its an incoming connection
			if (FD_ISSET(master_socket, &readfds))
			{
				if ((new_socket = accept(master_socket,(struct sockaddr *)&address, (socklen_t*)&addrlen))<0)
				{
					perror("accept");
					exit(EXIT_FAILURE);
				}
			
				// inform user of socket number - used in send and receive commands
				printf("New connection , socket fd is %d , ip is : %s , port : %d\n" , new_socket , inet_ntoa(address.sin_addr) , ntohs(address.sin_port));
			
				pthread_t pid;		
				if(pthread_create(&pid, NULL, handle_connection, (void*) &new_socket) != 0 )
				{
					std::cout << "Handshake Failure" << std::endl;
				}
				pthread_detach(pid);	
			}
			
			// else its some IO operation on some other socket
			for (i = 0; i < max_clients; i++)
			{
				pthread_mutex_lock(&mutex);
				sd = client_socket[i];
				pthread_mutex_unlock(&mutex);

				if (FD_ISSET( sd , &readfds))
				{
					// Check if it was for closing , and also read the
					// incoming message
					if ((valread = read( sd , buffer, 1024)) <= 0)
					{
						//Somebody disconnected , get his details and print
						getpeername(sd , (struct sockaddr*)&address , \
						(socklen_t*)&addrlen);
						printf("Host disconnected , ip %s , port %d \n" ,
						inet_ntoa(address.sin_addr) , ntohs(address.sin_port));
						
						//Close the socket and mark as 0 in list for reuse
						close( sd );
						client_socket[i] = 0;
					}
					// Echo back the message that came in
					else
					{
						send(sd , message , strlen(message) , 0 );
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
    if(nbytes < 0)
	{
   		close(newfd);
		pthread_exit(0);
	}
    if(nbytes == 0)
   	{  
		close(newfd);
        pthread_exit(0);
    }

    // print the clients name to the console
    printf("%s", buf);
    
    // clear our buffer out
	bzero(buf, BUFFER_SIZE);

    // send the client the list of possible spreadsheets
    char *file_list = "Beachhouse\nBroadcast\nBlouse\n\n";
    send(newfd, file_list, strlen(file_list), 0);
        
	// read the name of the spreadsheet the client wishes to edit
    nbytes = read(newfd, buf, BUFFER_SIZE);
    if(nbytes < 0)
    {	
		close(newfd);
  		pthread_exit(0);
	}
    if(nbytes == 0)
    {
       close(newfd);
       pthread_exit(0);
    }

    // print the name of the spreadsheet
    printf("%s", buf);
	
	//TODO: SEND STATE OF SPREADSHEET

	// add new socket to array of sockets
	for (int i = 0; i < sizeof(client_socket); i++)
	{
		pthread_mutex_lock(&mutex);
		int sd = client_socket[i];
		pthread_mutex_unlock(&mutex);
		//if position is empty in our array, fill it with the new socket
		if(sd == 0 )
		{
			pthread_mutex_lock(&mutex);
			client_socket[i] = newfd;
			pthread_mutex_unlock(&mutex);
			printf(" added to list of client sockets at index %d\n" , i);
			break;
		}
	}
	pthread_exit(0);
	// we are done, exit the thread
	//pthread_exit(0);
 	return 0;
}
