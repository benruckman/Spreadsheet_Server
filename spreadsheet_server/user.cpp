#include "user.h"

/*
 * TODO: Fill out documentation
 * negative ids are invalid.
 */
user::user(int buffer_size)
{
  this->id = -1;
  this->username = "";
  this->size = buffer_size;
  
  clear_buffer();
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
  return this->buffer;
}

void user::clear_buffer()
{
  clean();
  
  char* new_buffer = new char[size];
    
  for(int i = 0; i <= this->size; i++)
  {
    new_buffer[i] = 0;
  }
    
  this->buffer = new_buffer;
}

void user::clean()
{
  delete [] this->buffer;
}