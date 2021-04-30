/* Class that represents a spreadsheet user
 * Authors: Jackson McKay, John Richard, Soe Min Hitke, Donnie Kubiak, Jingwen Zang, Ben Ruckman
 * Date:    4/30/21
 */

#include "user.h"
#include <string>
#include <strings.h>

/* Creates an "invalid" default user
 * 
 * Parameters: None
 *  
 * Returns: Nothing
 */
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

/* Creates an new user
 * 
 * Parameters: id, socket, username, and spreadsheet name of this new user
 *  
 * Returns: A new user
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
 
/* Destroys a new user
 *
 * Parameters: None
 *  
 * Returns: Nothing
 */
user::~user()
{
  if(data != nullptr)
    free(data);
}

/* Returns spreadsheet this user is editing
 * 
 * Parameters: None
 *  
 * Returns: name of spreadsheet
 */
std::string user::get_ssname()
{
  return this->ssname;
}

/* Returns cell this user is editing
 * 
 * Parameters: None
 *  
 * Returns: name of cell
 */
std::string user::get_current()
{
	return this->currcell;
}

/* Sets the current cell this user is editing
 * 
 * Parameters: the name of the cell the user is editing
 *  
 * Returns: Nothing
 */
void user::set_current(std::string curr)
{
	this->currcell = curr;
}

/* Returns id of this user 
 * 
 * Parameters: None
 *  
 * Returns: id of user
 */
int user::get_id()
{
  return this->id;
}

/* Returns socket this user is connnected to
 * 
 * Parameters: None
 *  
 * Returns: socket of user
 */
int user::get_socket()
{
    return this->socket;
}

/* Returns username of this user
 * 
 * Parameters: None
 *  
 * Returns: username of user
 */
std::string user::get_username()
{
  return this->username;
}

/* Returns buffer of this cell a socket action may write to
 * 
 * Parameters: None
 *  
 * Returns: the buffer to read or write to
 */
char* user::get_buffer()
{
  return this->buffer;
}

/* Clears out all data from this users buffer
 * 
 * Parameters: None
 *  
 * Returns: Nothing
 */
void user::clear_buffer()
{
  bzero(this->buffer, 1024);
}

/* Returns this users unprocessed data
 * 
 * Parameters: None
 *  
 * Returns: string pointer of unprocessed data
 */
std::string* user::get_data()
{
  return this->data;
}

/* Adds a message to be processsed to data
 * 
 * Parameters: message to be processed
 *  
 * Returns: Nothing
 */
void user::add_data(std::string data)
{
  this->data->append(data);
}

/* Removes a message from data, at start to length
 * 
 * Parameters: index to start remove, lenght of remove
 *  
 * Returns: Nothing
 */
void user::remove_data(int start, int length)
{
   this->data->erase(start, length);
}
        