#include "user.h"
#include <string>
#include <strings.h>
user::user()
{
	this->id = -1;
	this->username = "";
	this->size = 0;
	this->socket = -1;
	this->ssname =""; 
	this->buffer = NULL;	
}
/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
user::user(int id, int socket, std::string username, std::string ssname)
{
  this->id = id;
  this->username = username;
  this->size = 1024;
  this->socket = socket;  
  this->ssname = ssname;
  this->buffer = new char[1024];
}

/*
 * TODO: Fill out documentation
 */
user::~user()
{
  clean();
}
/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
std::string user::get_ssname()
{
  return this->ssname;
}


/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
int user::get_id()
{
  return this->id;
}

/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
int user::get_socket()
{
    return this->socket;
}


/*
 * TODO: Fill out documentation
 */
std::string user::get_username()
{
  return this->username;
}

/*
 * TODO: Fill out documentation
 */
char* user::get_buffer()
{
  return this->buffer;
}

void user::clear_buffer()
{
  bzero(this->buffer, 1024);
}

void user::clean()
{
  delete [] this->buffer;
}

