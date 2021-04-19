#include "user.h"

/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
user::user(int buffer_size)
{
  this->id = -1;
  this->username = "";
  char temp_buffer[buffer_size];
  for(int i = 0; i < buffer_size; i++)
  {
    temp_buffer[i] = 0;
  }
  
  this->buffer = temp_buffer;
}

/*
 * TODO: Fill out documentation
 */
user::~user()
{
}

/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
int user::get_id()
{
  return id;
}

/*
 * TODO: Fill out documentation
 */
void user::set_id(int new_id)
{
  id = new_id;
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
void user::set_username(std::string new_username)
{
  this->username = new_username;
}
  
/*
 * TODO: Fill out documentation
 */
char* user::get_buffer()
{
  return buffer;
}
