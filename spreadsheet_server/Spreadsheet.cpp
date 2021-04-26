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
//#include <boost/algorithm/string.hpp>
#include "spreadsheet.h"
#include <set>
#include <iterator>
#include <algorithm>
#include <regex>
#include <tuple>
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
  
  map<string, string> cells;
  this->non_empty_cells = cells;
  
  queue<cell> history;
  this->cell_history = history;
<<<<<<< HEAD
    
  queue<tuple<string, string, string>> messages;
=======
  
  queue<string> messages;
>>>>>>> main
  this->message_queue = messages;
  
  vector<user> users;
  this->user_list = users;

  // Open the spreadsheet if it exists
  open_spreadsheet(name);
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
void spreadsheet::add_message(string new_message)
{
  // pthread_mutex_lock(&mutexqueue);
  message_queue.push(deserialize_message(new_message));
  // pthread_mutex_unlock(&mutexqueue);
  return;
}


/*
 * Reurns the contents of the named cell.
 *
 * @param name - the desired cell's name
 * @return the contents of the named cell
 */
string spreadsheet::get_cell_contents(string name)
{
  //googled stackoverflow for how to iterate through a map
  for(map<string, string>::iterator it = non_empty_cells.begin(); it != non_empty_cells.end(); it++)
  {
    //it->first is the key of the map
    if(it->first == name)
      return non_empty_cells[name];
  }
  return "";
}


/*
 * TODO: Document
 */
vector<string> spreadsheet::get_names_of_all_non_empty_cells()
{
  vector<string> cells;
  
  //this is the way to iterate through a map
  for(map<string, string>::iterator it = non_empty_cells.begin(); it != non_empty_cells.end(); it++)
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
  if(undo == false && non_empty_cells.size() != 0)
  {
    for(map<string, string>::iterator it = non_empty_cells.begin(); it != non_empty_cells.end(); it++)
    {
      //if the key in the map is equal to name of the cell
      if(it->first == name)
      {
	      //it->second is the value associated with the key
	      previous_contents = it->second;
	      set<string> newDependents;
	      get_dependency_graph().replaceDependents(name, newDependents);
      }
    }
    non_empty_cells[name] = content;
    //needs to check if the content is a formula, if it is, then we would need a way to return variable in the formula
    //then check for circular dependencies and replace dependents and add dependencies
    /* 
    if(content[0] == "=")
    {
      vector<string> var = get_variables(content);
      set<string> dependents;
      for(int i = 0; i < var.size(); i++)
      {
	dependents.insert(name, var[i]);
      }
      get_dependency_graph().addDependents(name, dependents);
      cell previousCell;
      previousCell.name = name;
      previousCell.content = previous_contents;
      cell_history.push();
      set_contents_of_cell(name, previous_contents, true);
    */
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
  for(vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
  {
    if(it->get_id() == id)
    {
      user_list.erase(it);
      break; // this fixes something, not sure why? -Jackson
    }
  }
}


/*
 * TODO: Document
 */
string spreadsheet::undo_spreadsheet()
{
  cell previous_cell = cell_history.front();
  
  cell_history.pop();
  
  string cell_name = previous_cell.cell_name;
  string cell_content = previous_cell.cell_contents;
  
  set_contents_of_cell(cell_name, cell_content, true);
  
  string new_change = cell_name + " : " + cell_content;
  
  return new_change;
}


/*
 * TODO: Document
 */
void spreadsheet::save()
{
  ofstream f;
  
  string file_name = "./../spreadsheet_data/" + spreadsheet_name + ".txt";
  
  //needs to pass in a c string to open the file
  f.open(file_name.c_str());
  
  //iterate through the nonempty cell and write them to a text file
  for(map<string, string>::iterator it = non_empty_cells.begin(); it != non_empty_cells.end(); it++)
  { 
    cout << "Hello from save" << endl;
    //it->first is the cell name in the map, it->second is the contents of the cell in the map
    string cells = it->first + " " + it->second + "\n";
    f<<cells;
  }
  
  f.close();
}


/*
 * TODO: Document
 */
map<string, string> spreadsheet::open_spreadsheet(string file_name)
{
  string sheet_name = file_name + ".txt";
  ifstream f;
  string cell_name = "";
  string cell_contents;
  
  // Open the file
  f.open(sheet_name.c_str());
  
  while(f>>cell_name>>cell_contents)
  {
    cout << cell_name << " " << cell_contents << endl;
    set_contents_of_cell(cell_name, cell_contents, true);
  }
  
  f.close();
  
  return non_empty_cells;
}


/*
 * TODO: Document
 */
string spreadsheet::normalize(string cell_contents)
{
  string normalized_contents;
  
  for(int i = 0; i < cell_contents.size(); i++)
  {
    normalized_contents += toupper(cell_contents[i]);
  }
  
  return normalized_contents;
}


/*
 * TODO: Document
 */
/*
bool spreadsheet::process_messages()
{

   std::cout <<"Beginning size: " << message_queue.size() << std::endl;
   pthread_mutex_lock(&mutexqueue);
  while(message_queue.size()> 0)
  {
    string message = message_queue.front();
    message_queue.pop();
    std::cout << message << std::endl;
    int n = message.length();
    // manipulate message if it is a valid request
    char m[n + 1];
    strcpy(m, message.c_str());
    for(vector<user>::iterator it = user_list.begin(); it != user_list.end(); it++)
    {
      std::cout << "sending message to user " << it->get_username() << std::endl;
      // process this message, and send good one back
      char* mess = "{\"messageType\" : \"cellUpdated\", \"cellName\" : \"a1\", \"contents\" : \"=1 + 3\"}\n";
      send(it->get_socket(), mess, strlen(mess), 0);
    }
  }
  pthread_mutex_unlock(&mutexqueue);
   std::cout <<"size: " << message_queue.size() << std::endl;
  return true;
}
*/
/*
 *TODO: Document
 */
vector<string> spreadsheet::get_variables(string contents)
{
  vector<string> input;
  vector<string> variables;
  //REFERENCE: geeksforgeeks.org/boostsplit-c-library/
  //boost::split(input, contents, boost::is_any_of("-|+|/|*|="));
  for(vector<string>::iterator it = input.begin(); it != input.end(); it++)
  {
    //REFERENCE: tutorialspoint.com/c_standard_library/c_function_atoi.htm
    //atoi returns 0 if the string is not converted to an integer, thus it is a variable
    int val = atoi((*it).c_str());
    if(val == 0)
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

/*
 *TODO: Document
 */
string spreadsheet::serialize_cell_update(string messageType, string cellName, string contents)
{
  string output = "{messageType: \"" + messageType + "\", cellName: " + cellName + ", contents: " + contents + "}";
  return output;
}
/*
 *TODO: Document
 */
string spreadsheet::serialize_cell_selected(string messageType, string cellName, int selector, string selectorName)
{
  string output = "{messageType: \"" + messageType + "\", cellName: " + cellName + ", selector: " + std::to_string(selector) + ", selectorName: " + selectorName + "}";
  return output;
} 
/*
 *TODO: Document
 */
string spreadsheet::serialize_disconnected(string messageType, int user)
{
  string output = "{messageType: \"" + messageType + "\", user: " + std::to_string(user) + "}";
  return output;
} 
/*
 *TODO: Document
 */
string spreadsheet::serialize_invalid_request(string messageType, string cellName, string message)
{
  string output = "{messageType: \"" + messageType + "\", cellName: " + cellName + ", message: " + message + "}";
  return output;
} 
/*
 *TODO: Document
 */
string spreadsheet::serialize_server_shutdown(string messageType, string message)
{
  string output = "{messageType: \"" + messageType + "\", message: " + message + "}";
  return output;
} 

/*
 *TODO: Document
 */
tuple<string, string, string> spreadsheet::deserialize_message(string input)
{
  tuple <string, string, string> result;
  // std::regex regex(",");
  //std::vector<std::string> firstOutput(std::sregex_token_iterator(input.begin(), input.end(), regex, -1), std::sregex_token_iterator());
  // std::regex another_regex(":");
  //std::vector<std::string> secondOutput(std::sregex_token_iterator(input.begin(), input.end(), another_regex, -1), std::sregex_token_iterator());
  vector<string> firstOutput = split(input, ',');
  string secondInput = "";
  for(int i = 0; i < firstOutput.size(); i++)
  {
    secondInput += firstOutput[i];
  }
  vector<string> secondOutput = split(secondInput, ':');
  if(secondOutput.size() == 6)
  {
    result = make_tuple(secondOutput[1], secondOutput[3], secondOutput[5].substr(0, secondOutput[5].length() - 1));
  }  
  if(secondOutput.size() == 4)
  {
    result = make_tuple(secondOutput[1], secondOutput[3].substr(0, secondOutput[3].length() - 1), NULL);
  }
  if(secondOutput.size() == 2)
  {
    result = make_tuple(secondOutput[1].substr(0, secondOutput[1].length() - 1), NULL, NULL);
  }
  return result;
} 

queue<tuple<string, string, string>> spreadsheet::get_message_queue()
{
  return this->message_queue;
}

vector<string> spreadsheet::split(string str, char delimeter)
{
  std::stringstream ss(str);
  string item;
  vector<string> splittedStrings;
  while(std::getline(ss, item, delimeter))
  {
    splittedStrings.push_back(item);
  }
    return splittedStrings;
}
