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
  this->data = nullptr;
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
  char buffer[1024];
  this->buffer = buffer;
  this->currcell = "A1";
  this->data = new std::string();
}
 
/*
 * TODO: Fill out documentation
 */
user::~user()
{
  //free(data);
}
/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
std::string user::get_ssname()
{
  return this->ssname;
}

std::string user::get_current()
{
	return this->currcell;
}

void user::set_current(std::string curr)
{
	this->currcell = curr;
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


std::string* user::get_data()
{
  return this->data;
}

void user::add_data(std::string data)
{
  this->data->append(data);
}

void user::remove_data(int start, int length)
{
   this->data->erase(start, length);
}
        