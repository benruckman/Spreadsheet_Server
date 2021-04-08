#include<stdio.h>
#include<stdlib.h>
#include<string.h>
#include<unistd.h>
#include<sys/types.h>
#include<sys/socket.h>
#include<netinet/in.h>
#include<pthread.h>
#include<string>

#define PORT 1100

using namespace std;

void accept_connections(int socket_fd, struct sockaddr * addr, socklen_t* addrlen);

main(int argc, char const *argv[])
{
  // Initialize variables
  int socket_fd;
  int chars_read;
  int new_socket;
  char buffer[1024] = {0};
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
  
  // Binding the socket to the host address
  if(bind(socket_fd, (struct sockaddr *) &server_address, sizeof(server_address)) < 0)
  {
    perror("Binding the socket failed.");
    exit(EXIT_FAILURE);
  }

  // Begins listening for new connections
  if(listen(socket_fd, 5) < 0)
  {
    perror("Error while listening.");
    exit(EXIT_FAILURE);
  }
 
  // Accepts a new connection
  if ((new_socket = accept(socket_fd, (struct sockaddr *) &server_address, (socklen_t *) &address_length)) < 0)
  {
    perror("Error while accepting.");
    exit(EXIT_FAILURE);
  }

  // Read the first message from client. Message should be username
  chars_read = read(new_socket, buffer, 1024);
  printf("%s\n", buffer);
 
  string username = "";
  char current =  buffer[0];
  int i = 0;
  while(current != '\n')
  {
    username += current;
    i++;
    current = buffer[i];
  }

  // Send a list of file names
  char *file_list= "Spreadsheet1\nDonnieDarko\nVinylRecordsAreCool\n\n";
  send(new_socket, file_list, sizeof(file_list), 0);
  chars_read = read(new_socket, buffer, 1024);
  printf("%s\n", buffer);

  // Read the second message from client. Message should be the file name
  string file_name = "";
  current = buffer[0];
  i = 0;
  while(current != '\n')
  {
    file_name += current;
    i++;
    current = buffer[i];
  }

  // Still learning about multithreading. We may uncomment in the final version
  //pthread_t t;  
  //pthread_create(t, NULL, accept_connections, socket_fd, (struct sockaddr *) &server_address, (socklen_t*) sizeof(server_address));
  sleep(5000);
  return 0;
}

/*
 * Method is used to accept clients on its own thread. This method is currently not used.
 */
void accept_connections(int socket_fd, struct sockaddr * addr, socklen_t* addrlen)
{
  int result;
  while(true)
  {
    if(result = accept(socket_fd, addr, addrlen) < 0)
    {
      perror("Accepting the connection failed.");
      exit(EXIT_FAILURE);
    }
  }
}
