/* Class that represents the state of a networked spreadsheet
 * Authors: Jackson McKay, John Richard, Soe Min Hitke, Donnie Kubiak, Jingwen Zang, Ben Ruckman
 * Date:    4/30/21
 */

#include <iostream> 
#include <unistd.h> 
#include <arpa/inet.h> 
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <sys/time.h> 
#include <string>
#include <string.h>
#include <stdio.h>
#include <bits/stdc++.h>
#include "spreadsheet.h"
#include <set>
#include <iterator>
#include <algorithm>
#include <regex>
#include <tuple>
#include <map>
#include <stack>

// Use the standard library for ease of accessing functions and classes
using namespace std;

// A lock to ensure critical sections of code, like the message queue, remains intact
pthread_mutex_t mutexqueue = PTHREAD_MUTEX_INITIALIZER;

/* Constructor: Creates a spreadsheet.
 * If the name matches an existing spreadsheet text file, that file is opened and
 * the contents are loaded into this spreadsheet. If the file doesn't exist, a new,
 * empty spreadsheet is created.
 */
spreadsheet::spreadsheet(string name)
{
	this->spreadsheet_name = name;
	
	map <string, cell > cells;
	this->non_empty_cells = cells;

	map<string, queue<string>> cellHistory;
	this->cell_history = cellHistory;

	queue<message> messages;
	this->message_queue = messages;
	
	vector<user> users;
	this->user_list = users;
	
	stack<edit>* history_real = new stack<edit>;
	this->history_real = history_real;
	
	open_spreadsheet(name);
}

/* Default constructor, required for placement of spreadsheets into unordered maps
 * Left empty intentionally, do not call
 */
spreadsheet::spreadsheet()
{

}

/* Destructor: Deletes and frees all heap allocated memory
 * Required by convention
 */
spreadsheet::~spreadsheet()
{
	int num_users = this->user_list.size();
	
	for (int i = 0; i < num_users; i++)
	{
		delete &user_list[i];
	}
	
	delete history_real;
}

/* Returns the spreadsheet name
 * 
 * Parameters: this 
 *
 * Returns: the name of this spreadsheet
 */
string spreadsheet::get_name()
{
	return this->spreadsheet_name;
}

/* Sets the spreadsheet name
 * 
 * Parameters: this, the name of the spreadsheet
 *
 * Returns: Nothing
 */
void spreadsheet::set_name(string name)
{
	this->spreadsheet_name = name;
}

/* Adds a request to change this spreadsheet to queue to be processed
 * 
 * Parameters: this, the message to be processed, and the ID of the client making the request
 *
 * Returns: Nothing
 */
void spreadsheet::add_message(string new_message, int ID)
{
	pthread_mutex_lock(&mutexqueue);
	message m = deserialize_message(new_message);
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		if (it->get_id() == ID)
			m.sender = &(*it);
	}
	message_queue.push(m);
	pthread_mutex_unlock(&mutexqueue);
	return;
}

/* Returns the map of non-empty cells of this spreadsheet
 * 
 * Parameters: this 
 *
 * Returns: A mapping of cell names to cells
 */
map<string, spreadsheet::cell> spreadsheet::get_non_empty_cells()
{
	return non_empty_cells;
}

/* Returns the cell corresponding to a cell name
 * 
 * Parameters: this, and the name of the cell 
 *
 * Returns: The cell represented by the name
 */
spreadsheet::cell spreadsheet::get_cell_contents(string name)
{	
	auto it = non_empty_cells.find(name);
	// If we found the cell in the list
	if(it != non_empty_cells.end())
		return it->second;
	// Otherwise, it is empty, return an empty cell
	else
	{
		cell c;
		c.cell_name = name;
		std::stack<string> history;
		c.cell_contents = history;
		return c;
	}
}

/* Returns names of non empty cells
 * 
 * Parameters: this
 *
 * Returns: A vector containing the names of all of this spreadsheets non-empty cells
 */
vector<string> spreadsheet::get_names_of_all_non_empty_cells()
{
	vector<string> cells;
	for (map<string, cell>::iterator it = get_non_empty_cells().begin(); it != get_non_empty_cells().end(); it++)
	{
		cells.push_back(it->first);
	}
	return cells;
}

/* Sets the contents of a cell
 * 
 * Parameters: this, the name of the cell to be edited, the new content, 
 * 		and whether or not this action is an undo 
 *
 * Returns: The cell represented by the name
 */
bool spreadsheet::set_contents_of_cell(string name, string content, bool undo)
{
	// used to keep the previous contents of a cell
	string previous_contents = "";
	auto it = non_empty_cells.find(name);
	if (it != non_empty_cells.end())
	{
		// get the cell in question
		cell* c = &it->second;
		previous_contents = c->cell_contents.top();
		if (content[0] == '=') // this is a formula
		{
			vector<string> var = get_variables(content);
			if (creates_circular_dependency(name, var)) {
				std::cout << "circular dependency" << std::endl;
				return false;
			}
			set<string> newDependents;
			for (int i = 0; i < var.size(); i++)
			{
				newDependents.insert(var[i]);
			}
			get_dependency_graph().replaceDependents(name, newDependents);
		}
		if (!undo)
		{
			c->cell_contents.push(content);
			edit e;
			e.name = name;
			e.contents = content;
			this->history_real->push(e);
		}
		non_empty_cells[name] = *c;
		return true;
	}
	else
	{
		if (content[0] == '=') // this is a formula
		{
			vector<string> var = get_variables(content);
			if (creates_circular_dependency(name, var)) {
				std::cout << "circular dependency" << std::endl;
				return false;
			}
			for (int i = 0; i < var.size(); i++)
			{
				get_dependency_graph().addDependency(name, var[i]);
			}
		}
		cell c;
		c.cell_name = name;
		c.cell_contents.push(content);
		non_empty_cells[name] = c;
		edit e;
		e.name = name;
		e.contents = content;
		this->history_real->push(e);
		return true;
	}
}

/* Checks to see whether a setting the contents of a cell would create a circular dependency
 * 
 * Parameters: this, the name of the cell, and a list of the cells variables in its contents
 *
 * Returns: True if it creates a dependency, false otherwise
 */
bool spreadsheet::creates_circular_dependency(const string name, const vector<string>& vars) {
	for (int i = 0; i < vars.size(); i++) {
		if (get_dependency_graph().hasDependents(vars[i])) {
			set<string> var_dependents = get_dependency_graph().getDependents(vars[i]);
			for (set<string>::iterator i = var_dependents.begin(); i != var_dependents.end(); i++) {
				if (*i == name) {
					return true;
				}
			}
		}
	}
	return false;
}

/* Adds a new user to this spreadsheet
 * 
 * Parameters: this, and the user to add
 *
 * Returns: Nothing
 */
void spreadsheet::add_user(user* new_user)
{
	user_list.push_back(*new_user);
}


/* Removes a user from this spreadsheet
 * 
 * Parameters: this, and the user to remove
 *
 * Returns: Nothing
 */
void spreadsheet::remove_user(int id)
{
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		if (it->get_id() == id)
		{
			user_list.erase(it);
			break; 
		}
	}
}


std::stack<std::string> spreadsheet::get_spreadsheet_history()
{
	return this->spreadsheet_history;
}


map<string, queue<string>> spreadsheet::get_cell_history()
{
	return this->cell_history;
}


/* Reverts the contents of a cell to what was previously held in it
 * 
 * Parameters: this, and the name of the cell to revert
 *
 * Returns: the name and contents of the newly reverted cell, separated by a space
 */
string spreadsheet::revert_cell(string selectedCell)
{
	// Find the cell we are reverting
	auto it = non_empty_cells.find(selectedCell);
	string ret = selectedCell + " ";
	if (it != non_empty_cells.end())
	{
		cell* c = &it->second;
		if (c->cell_contents.empty())
		{
			// If the cell has no past, return empty contents
			return ret;
		}
		else
		{
			// Find out what used to be in the cell
			string old = c->cell_contents.top();
			c->cell_contents.pop();
			c->current_reverts.push(old);
			if (c->cell_contents.empty())
			{
				// TODO: destroy dependencies
				edit e;
				e.name = selectedCell;
				e.contents = "";
				this->history_real->push(e);
				//non_empty_cells.erase(selectedCell);
				return ret;
			}
			else
			{
				// TODO: check for dependencies
				string neww = c->cell_contents.top();
				edit e;
				e.name = selectedCell;
				e.contents = neww;
				this->history_real->push(e);
				ret = selectedCell + " " + neww;
				return ret;
			}
		}
	}
	else
	{
		ret = "";
	}
	return ret;
}

/* Undos the last edit made to this spreadsheet
 * 
 * Parameters: this
 *
 * Returns: A string of the cell name and its new contents after an undo
 */
string spreadsheet::undo()
{
	if (!history_real->empty())
	{
		edit e = history_real->top();
		string last = e.name;
		auto it = non_empty_cells.find(last);
		string ret = last + " ";
		if (it != non_empty_cells.end())
		{
			cell* c = &it->second;
			if (!c->current_reverts.empty())
			{
				string s = c->current_reverts.top();
				c->current_reverts.pop();
				c->cell_contents.push(s);
				ret = e.name + " " + c->cell_contents.top();
			}
			else if (!c->cell_contents.empty())
			{
				c->cell_contents.pop();
				ret = e.name + " " + c->cell_contents.top();
			}
		}
		history_real->pop();
		return ret;
	}
	return "";
}

/* Saves this spreadsheet to a text file
 * 
 * Parameters: this
 *
 * Returns: Nothing
 */
void spreadsheet::save()
{
	// TODO: ABSOLUTE FILE NAME
	string file_name = "/spreadsheet_data/" + spreadsheet_name + ".sprd";
	//string file_name =  spreadsheet_name + ".txt";

	ofstream f(file_name);
	// Iterate through the nonempty cell and write them to a text file
	stack<edit> reverse_history;
	while (!this->history_real->empty())
	{
		edit e = this->history_real->top();
		reverse_history.push(e);
		this->history_real->pop();
	}
	while (!reverse_history.empty())
	{
		edit e = reverse_history.top();
		string cells = e.name + " " + e.contents + "\n";
		f << cells;
		reverse_history.pop();
	}
	f.close();

}


/* Opens and writes the contents of a spreadsheet file to this spreadsheet
 * 
 * Parameters: this, the filename of the spreadsheet to read from
 *
 * Returns: Nothing
 */
void spreadsheet::open_spreadsheet(string file_name)
{
	string sheet_name = "/spreadsheet_data/" + file_name + ".sprd";
	ifstream f;
	string cell_name = "";
	string cell_contents = "";

	// Parse our data from our file
	std::ifstream file(sheet_name);
	for (std::string line; std::getline(file, line);)
	{
		size_t last_index = line.find_first_of(" ");
		cell_name = line.substr(0, last_index);
		cell_contents = line.substr(cell_name.length() + 1);
		set_contents_of_cell(cell_name, cell_contents, false);	
	}
	file.close();
}


/* "Normalizes" the contents of a cell, i.e. brings it to uppercase
 * 
 * Parameters: this, the contents of a cell
 *
 * Returns: the normalized contents
 */
string spreadsheet::normalize(string cell_contents)
{
	string normalized_contents;
	for (int i = 0; i < cell_contents.size(); i++)
	{
		normalized_contents += toupper(cell_contents[i]);
	}
	return normalized_contents;
}


/* Attempts to process all requests currently in the queue,
 * 		and sends the processed messages back to all clients of this spreadsheet
 * 
 * Parameters: this
 *
 * Returns: False if one of the messages in the queue could not be processed, True otherwise
 */
bool spreadsheet::process_messages()
{
	pthread_mutex_lock(&mutexqueue);
	while (message_queue.size() > 0)
	{
		message m = message_queue.front();
		this->message_queue.pop();
		string message;
		if (m.type == "editCell")
		{
			if (!set_contents_of_cell(m.name, m.contents, false))
			{
				message = serialize_invalid_request("requestError", m.name, "Circular dependency created.");
				std::cout << "circular dependency" << std::endl;
			}
				
			else
				message = serialize_cell_update("cellUpdated", m.name, m.contents);
		}
		else if (m.type == "selectCell")
		{
			message = serialize_cell_selected("cellSelected", m.name, m.sender->get_id(), m.sender->get_username());
			m.sender->set_current(m.name);
		}
		else if (m.type == "undo")
		{
			string line = undo();
			if (line != "")
			{
				size_t last_index = line.find_first_of(" ");
				std::string cell_name = line.substr(0, last_index);
				std::string cell_contents = line.substr(cell_name.length() + 1);
				message = serialize_cell_update("cellUpdated", cell_name, cell_contents);
			}
		}
		else if (m.type == "revertCell")
		{
			string line = revert_cell(m.name);
			if (line != "")
			{
				size_t last_index = line.find_first_of(" ");
				std::string cell_name = line.substr(0, last_index);
				std::string cell_contents = line.substr(cell_name.length() + 1);
				message = serialize_cell_update("cellUpdated", cell_name, cell_contents);
			}
		}
		else
		{
			pthread_mutex_unlock(&mutexqueue);
			return false;
		}
		int n = message.length();
		char mess[n + 1];
		strcpy(mess, message.c_str());
		for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
		{
			send(it->get_socket(), mess, strlen(mess), 0);
		}
	}
	pthread_mutex_unlock(&mutexqueue);

	return true;
}


/* Returns all variables from a cells contents
 * 
 * Parameters: this, and the cells contents
 *
 * Returns: Nothing
 */
vector<string> spreadsheet::get_variables(string contents)
{
	vector<string> input;
	vector<string> variables;
	string inputString = contents;
	char delimeters[5] = { '-', '+', '*', '/', '=' };
	for (int i = 0; i < 5; i++)
	{
		input = split(inputString, delimeters[i]);
		inputString = "";
		for (int j = 0; j < input.size(); j++)
		{
			inputString += input[j];
		}
	}
	for (vector<string>::iterator it = input.begin(); it != input.end(); it++)
	{
		//REFERENCE: tutorialspoint.com/c_standard_library/c_function_atoi.htm
		//atoi returns 0 if the string is not converted to an integer, thus it is a variable
		int val = atoi((*it).c_str());
		if (val == 0 && ((*it).c_str() != "0"))
		{
			variables.push_back(*it);
		}
	}
	return variables;
}


/* Returns this spreadsheets dependency graph
 * 
 * Parameters: this
 *
 * Returns: The dependency graph object of this spreadsheet
 */
DependencyGraph spreadsheet::get_dependency_graph()
{
	return g;
}

/* Sends the entire current state of this spreadsheet to a socket
 * 
 * Parameters: this, and the socket to send to
 *
 * Returns: Nothing
 */
void spreadsheet::send_spreadsheet(int socket)
{
	for (map<string, cell>::iterator it = non_empty_cells.begin(); it != non_empty_cells.end(); it++)
	{
		std::string s = serialize_cell_update("cellUpdated", it->first, it->second.cell_contents.top());
		int n = s.length();
		char message[n + 1];
		strcpy(message, s.c_str());
		send(socket, message, strlen(message), 0);
	}
}

/* Sends a disconnection message of ID to all connected clients of this spreadsheet
 * 
 * Parameters: this, ID of disconnected user
 *
 * Returns: Nothing
 */
void spreadsheet::send_disconnect(int ID)
{
	std::string s = serialize_disconnected("disconnected", ID);
	int n = s.length();
	char message[n + 1];
	strcpy(message, s.c_str());
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		send(it->get_socket(), message, strlen(message), 0);
	}
}

/* Sends current selections of this spreadsheet to socket socket
 * 
 * Parameters: this, and the socket to send to
 *
 * Returns: Nothing
 */
void spreadsheet::send_selections(int socket)
{
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		std::string s = serialize_cell_selected("cellSelected", it->get_current(), it->get_id(), it->get_username());
		int n = s.length();
		char message[n + 1];
		strcpy(message, s.c_str());
		send(socket, message, strlen(message), 0);
	}
}


/* Serializes a cell update message into proper JSON
 * 
 * Parameters: this, the messageType, the cellname, and the cell contents
 *
 * Returns: The JSON message
 */
string spreadsheet::serialize_cell_update(string messageType, string cellName, string contents)
{
	string output = "{\"messageType\": \"" + messageType + "\", \"cellName\": \"" + cellName + "\", \"contents\": \"" + contents + "\"}\n";
	return output;
}


/* Serializes a cell select message into proper JSON
 * 
 * Parameters: this, the messageType, the cellname, the selector, and the selector name
 *
 * Returns: The JSON message
 */
string spreadsheet::serialize_cell_selected(string messageType, string cellName, int selector, string selectorName)
{
	string output = "{\"messageType\": \"" + messageType + "\", \"cellName\": \"" + cellName + "\", \"selector\":"  + std::to_string(selector) + ", \"selectorName\": \"" + selectorName + "\"}\n";
	return output;
}


/* Serializes a cell disconnect message into proper JSON
 * 
 * Parameters: this, the messageType, and the user ID of the disconnected user
 *
 * Returns: The JSON message
 */
string spreadsheet::serialize_disconnected(string messageType, int user)
{
	string output = "{\"messageType\": \"" + messageType + "\", \"user\": " + std::to_string(user) + "}\n";
	return output;
}


/* Serializes a cell invalid request message into proper JSON
 * 
 * Parameters: this, the messageType, the cellname, and the invalid request reason
 * Returns: The JSON message
 */
string spreadsheet::serialize_invalid_request(string messageType, string cellName, string message)
{
	string output = "{\"messageType\": \"" + messageType + "\", \"cellName\": \"" + cellName + "\", \"message\": \"" + message + "\"}\n";
	return output;
}


/* Serializes a server shutdown message to proper JSON
 * 
 * Parameters: this, the messageType, and the message
 *
 * Returns: The JSON message
 */
string spreadsheet::serialize_server_shutdown(string messageType, string message)
{
	string output = "{\"messageType\": \"" + messageType + "\", \"message\": \"" + message + "\"}\n";
	return output;
}


/* Deserializes a JSON cell request into a message the spreadsheet can read
 * 
 * Parameters: this, the JSON message to deserialize
 *
 * Returns: The JSON message
 */
spreadsheet::message spreadsheet::deserialize_message(string input)
{
	message result;
	string tester = "\"";
	int start = 0;
	string outputs[6]{ "","","","","","" };

	int i = 0;
	while (input.find(tester, start) != string::npos)
	{
		int findPos = input.find(tester, start); //finds first quote mark
		int findPos2 = input.find(tester, findPos + 1); //finds second quote mark
		std::string name;
		string neww = input.substr(findPos + 1, (findPos2 - findPos) - 1);
		outputs[i] = neww;
		start = findPos2 + 1;
		i++;
	}
	result.type = outputs[1];
	result.name = outputs[3];
	result.contents = outputs[5];
	return result;
}

/* Splits a string with a delimiter
 * 
 * Parameters: this, the string to split, and the delimiter
 *
 * Returns: A vector of the split items
 */
vector<string> spreadsheet::split(string str, char delimeter)
{
	std::stringstream ss(str);
	string item;
	vector<string> splittedStrings;

	while (std::getline(ss, item, delimeter))
	{
		splittedStrings.push_back(item);
	}
	return splittedStrings;
}