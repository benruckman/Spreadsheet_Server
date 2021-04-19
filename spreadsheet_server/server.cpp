#include "user.h"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <thread>
#include <string>
#include <map>

#define PORT 1100
#define BUFFER_SIZE 1024

void handle_connect(int socket_id);

main(int argc, char const *argv[])
{
  // Initialize variables
  int socket_fd;
  int chars_read;
  int new_socket;
  char buffer[BUFFER_SIZE] = {0};
  struct sockaddr_in server_address;
  int address_length = sizeof(server_address);

  // Create socket
  socket_fd = socket(AF_INET, SOCK_STREAM, 0);

  // Check for error
  if(socket_fd < 0)
  {
    perror("Socket failed to open.");
    exit(EXIT_FAILURE);
  }
  
  // Still learning what setsocketopt does. We may uncomment in the final version
  // setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR | SO_REUSEPORT, &opt, sizeof(opt))

  // Set up server address structure
  server_address.sin_family = AF_INET;
  server_address.sin_addr.s_addr = INADDR_ANY;
  server_address.sin_port = htons( PORT );
  std::cout<<"Binding the socket"<<std::endl;
  // Binding the socket to the host address
  if(bind(socket_fd, (struct sockaddr *) &server_address, sizeof(server_address)) < 0)
  {
    perror("Binding the socket failed.");
    exit(EXIT_FAILURE);
  }
  std::cout<<"Starts listening"<<std::endl;
  // Begins listening for new connections
  if(listen(socket_fd, 5) < 0)
  {
    perror("Error while listening.");
    exit(EXIT_FAILURE);
  }

  std::cout<<"Entering while-loop"<<std::endl;
  while(true)
  {
    // Process updates, notifying all clients of edits
    // Accepts a new connection
    if ((new_socket = accept(socket_fd, (struct sockaddr *) &server_address, (socklen_t *) &address_length)) < 0)
    {
      perror("Error while accepting.");
      exit(EXIT_FAILURE);
    }
    
    std::cout << "Hello from main" << std::endl;
    std::thread t(handle_connect, new_socket);
    t.detach();
  }
  












/*
  // Read the first message from client. Message should be username
  chars_read = read(new_socket, buffer, 1024);
 
  


  
  // Still learning about multithreading. We may uncomment in the final version
  //pthread_t t;  
  //pthread_create(t, NULL, accept_connections, socket_fd, (struct sockaddr *) &server_address, (socklen_t*) sizeof(server_address));
  sleep(5000);
  */
  
  return 0;
}

/*
 * Method is used to complete the handshake on its own thread.
 * 
 */
void handle_connect(int socket_id)
{
  std::cout << socket_id << std::endl;
  
  user new_user(BUFFER_SIZE);
  new_user.set_id(socket_id);
 
  int num_char = read(new_user.get_id(), new_user.get_buffer(), BUFFER_SIZE);
  // printf("%s\n", new_user.get_buffer());
  for(int j = 0; j < BUFFER_SIZE; j++)
  {
    std::cout<<new_user.get_buffer()[j];
  } 
  std::string username = "";
  char current =  new_user.get_buffer()[0];
  int i = 0;

  while(current != '\n')
  {
    username += current;
    i++;
    current = new_user.get_buffer()[i];
  }

  new_user.set_username(username);

  // Send a list of file names
  char *file_list= "Spreadsheet1\nDonnieDarko\nVinylRecordsAreCool\n\n";
  //send(new_user.get_id(), file_list, sizeof(file_list), 0);
  num_char = read(new_user.get_id(), new_user.get_buffer(), 1024);
  // printf("%s\n", new_user.get_buffer());

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
  
}
