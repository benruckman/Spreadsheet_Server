/* Class that represents a spreadsheet user
 * Authors: Jackson McKay, John Richard, Soe Min Hitke, Donnie Kubiak, Jingwen Zang, Ben Ruckman
 * Date:    4/30/21
 */

#include "user.h"
#include <string>
#include <strings.h>
#include <iostream>

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
	this->socket = -1;
	this->ssname =""; 
	this->buffer = NULL;	
  this->data = "";
  this->sizeleft = 0;
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
  this->socket = socket;  
  this->ssname = ssname;
  char buffer[1024];
  this->buffer = buffer;
  this->currcell = "A1";
  this->data = "";
  this->sizeleft = 0;
}
 
/* Destroys a new user
 *
 * Parameters: None
 *  
 * Returns: Nothing
 */
user::~user()
{
  //delete [] buffer;
}

/* Returns size of data not parse from buffer
 * 
 * Parameters: None
 *  
 * Returns: size of data not parse
 */
int user::get_sizeleft()
{
  return this->sizeleft;
}

/* Sets size of data not read from buffer
 * 
 * Parameters: size of data not read
 *  
 * Returns: Nothing
 */
void user::set_sizeleft(int s)
{
  this->sizeleft = s;
}

/* Sets our buffer to a new one
 * 
 * Parameters: our char array to copy from
 *  
 * Returns: nothing
 */
void user::set_buffer(char c[])
{
  this->buffer = c;
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

/* Returns the name of the cell the user is currently selecting
 * 
 * Parameters: None
 *  
 * Returns: the name of the cell the user is currently selecting
 */
std::string user::get_current_cell()
{
  return this->currcell;
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


        