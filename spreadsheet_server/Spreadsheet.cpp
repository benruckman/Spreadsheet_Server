/*
 Definitions for the spreadsheet class
 */
#include <iostream> // For testing purposes. TODO: remove from final version
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

 // used to lock critical sections of code, like the queue
pthread_mutex_t mutexqueue = PTHREAD_MUTEX_INITIALIZER;
using namespace std;
/*
 * Constructor: Creates a spreadsheet.
 * If the name matches an existing spreadsheet text file, that file is opened and
 * the contents are loaded into this spreadsheet. If the file doesn't exist, a new,
 * empty spreadsheet is created.
 */
spreadsheet::spreadsheet(string name)
{
	// Initialize variables
	this->spreadsheet_name = name;
	map <string, cell > cells;
	this->non_empty_cells = cells;
	map<string, queue<string>> cellHistory;
	this->cell_history = cellHistory;
	std::stack<string> history;
	this->spreadsheet_history = history;
	queue<message> messages;
	this->message_queue = messages;
	vector<user> users;
	this->user_list = users;
	open_spreadsheet(name);
	stack<edit> history_real;
	this->history_real = history_real;
}


spreadsheet::spreadsheet()
{

}


/*
 * Destructor: Deletes and frees all heap allocated memory
 */
spreadsheet::~spreadsheet()
{

}


/*
 * Returns the spreadsheet name

 * @return the spreadsheet name
 */
string spreadsheet::get_name()
{
	return this->spreadsheet_name;
}


/*
 * Sets the spreadsheet name to the provided string

 * @param name - The new spreadsheet name
 */
void spreadsheet::set_name(string name)
{
	this->spreadsheet_name = name;
}


/*
 * Adds a message to the queue of messages to process.
 *
 * @param new_message - the JSON string representing a new message request
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

/*
 *TODO: DOCUMENT
 */
map<string, spreadsheet::cell> spreadsheet::get_non_empty_cells()
{
	return non_empty_cells;
}

/*
 * Reurns the contents of the named cell.
 *
 * @param name - the desired cell's name
 * @return the contents of the named cell
 */
spreadsheet::cell spreadsheet::get_cell_contents(string name)
{
	//googled stackoverflow for how to iterate through a map
	for (map<string, cell>::iterator it = get_non_empty_cells().begin(); it != get_non_empty_cells().end(); it++)
	{
		//it->first is the key of the map
		if (it->first == name)
			return non_empty_cells[name];
	}
	cell c;
	c.cell_name = name;
	std::stack<string> history;
	c.cell_contents = history;
	return c;
}


/*
 * TODO: Document
 */
vector<string> spreadsheet::get_names_of_all_non_empty_cells()
{
	vector<string> cells;

	//this is the way to iterate through a map
	for (map<string, cell>::iterator it = get_non_empty_cells().begin(); it != get_non_empty_cells().end(); it++)
	{
		cells.push_back(it->first);
	}

	return cells;
}


/*
 * TODO: Document
 */
bool spreadsheet::set_contents_of_cell(string name, string content, bool undo)
{
	//used to keep the previous contents of a cell
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
			//spreadsheet_history.push(name);
			edit e;
			e.name = name;
			e.contents = content;
			history_real.push(e);
		}
		non_empty_cells[name] = *c;
		return true;
	}
	else
	{

		if (content[0] == '=') // this is a formula
		{
			vector<string> var = get_variables(content);
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
		history_real.push(e);
		spreadsheet_history.push(name);

		return true;
	}
}

/*
 * TODO: Document
 */
void spreadsheet::add_user(user* new_user)
{
	user_list.push_back(*new_user);
}

/*
 * TODO: Document
 */
void spreadsheet::remove_user(int id)
{
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		if (it->get_id() == id)
		{
			user_list.erase(it);
			break; // this fixes something, not sure why? -Jackson
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

/*
 * TODO: Document
 */
string spreadsheet::revert_cell(string selectedCell)
{
	auto it = non_empty_cells.find(selectedCell);
	string ret = selectedCell + " ";
	if (it != non_empty_cells.end())
	{
		cell* c = &it->second;
		if (c->cell_contents.empty())
		{
			return ret;
		}
		else
		{
			string old = c->cell_contents.top();
			c->cell_contents.pop();
			string new = c->cell_contents.top();

			if (c->cell_contents.empty())
			{
				c->cell_contents.push("");
				set_contents_of_cell(selectedCell, "", false);
				return ret;
			}
			else
			{
				string newc = c->cell_contents.top();
				ret = selectedCell + " " + newc;
				set_contents_of_cell(selectedCell, newc, false);
				return ret;
			}
		}
	}
	else
	{
		ret = "";
	}
	std::cout << ret << " was returned " << std::endl;
	return ret;
}

string spreadsheet::undo()
{
	if (!history_real.empty())
	{
		edit e = history_real.top();
		string last = e.name;
		auto it = non_empty_cells.find(last);
		string ret = last + " ";
		if (it != non_empty_cells.end())
		{
			cell* c = &it->second;
			if (c->cell_contents.empty())
			{
			}
			else
			{
				c->cell_contents.pop();
				if (c->cell_contents.empty())
				{
	
				}
				else
				{
					string newc = c->cell_contents.top();
					set_contents_of_cell(last, newc, true);
					ret = last + " " + newc;
				}
			}
		}
		else
		{
		}
		history_real.pop();
		return ret;
	}
	return "";
}

/*
 * TODO: Document
 */
void spreadsheet::save()
{
	ofstream f;

	string file_name = "./../spreadsheet_data/" + spreadsheet_name + ".txt";
	//string file_name =  spreadsheet_name + ".txt";
	//needs to pass in a c string to open the file
	f.open(file_name.c_str());

	//iterate through the nonempty cell and write them to a text file
	stack<edit> reverse_history;
	while (!history_real.empty())
	{
		//it->first is the cell name in the map, it->second is the contents of the cell in the map
		edit e = history_real.top();
		reverse_history.push(e);
		history_real.pop();
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

/*
 * TODO: Document
 */
void spreadsheet::open_spreadsheet(string file_name)
{
	string sheet_name = "./../spreadsheet_data/" + file_name + ".txt";
	ifstream f;
	string cell_name = "";
	string cell_contents = "";

	// parse our data from our file
	std::ifstream file(sheet_name);
	for (std::string line; std::getline(file, line);)
	{
		size_t last_index = line.find_first_of(" ");
		cell_name = line.substr(0, last_index);
		cell_contents = line.substr(cell_name.length() + 1);
		set_contents_of_cell(cell_name, cell_contents, false);
	}
}

/*
 * TODO: Document
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

/*
 * TODO: Document
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
			message = serialize_cell_update("cellUpdated", m.name, m.contents);
			set_contents_of_cell(m.name, m.contents, false);
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

/*
 *TODO: Document
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

/*
 *TODO:Document
 */
DependencyGraph spreadsheet::get_dependency_graph()
{
	return g;
}

// sends the entire spreadsheet to a socket
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

void spreadsheet::send_disconnect(int ID)
{
	std::string s = serialize_disconnected("disconnected", ID);
	int n = s.length();
	char message[n + 1];
	strcpy(message, s.c_str());
	for (vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
	{
		std::cout << message << std::endl;
		send(it->get_socket(), message, strlen(message), 0);
	}
}

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

/*
 *TODO: Document
 */
string spreadsheet::serialize_cell_update(string messageType, string cellName, string contents)
{
	string output = "{\"messageType\" : \"" + messageType + "\", \"cellName\" : \"" + cellName + "\", \"contents\" : \"" + contents + "\"}\n";
	return output;
}

/*
 *TODO: Document
 */
string spreadsheet::serialize_cell_selected(string messageType, string cellName, int selector, string selectorName)
{
	string output = "{\"messageType\" : \"" + messageType + "\", \"cellName\" : \"" + cellName + "\", \"selector\" : \"" + std::to_string(selector) + "\", \"selectorName\" : \"" + selectorName + "\"}\n";
	return output;
}

/*
 *TODO: Document
 */
string spreadsheet::serialize_disconnected(string messageType, int user)
{
	string output = "{\"messageType\" : \"" + messageType + "\", \"user\" :" + std::to_string(user) + "}\n";
	return output;
}

/*
 *TODO: Document
 */
string spreadsheet::serialize_invalid_request(string messageType, string cellName, string message)
{
	string output = "{\"messageType\" : \"" + messageType + "\", \"cellName\" : \"" + cellName + "\", \"message\" : \"" + message + "\"}\n";
	return output;
}

/*
 *TODO: Document
 */
string spreadsheet::serialize_server_shutdown(string messageType, string message)
{
	string output = "{\"messageType\" : \"" + messageType + "\", \"message\" : \"" + message + "\"}\n";
	return output;
}

/*
 *TODO: Document
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

