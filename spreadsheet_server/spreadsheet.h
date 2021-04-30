//needs to create DependencyGraph class to use its helper functions

#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <iostream>
#include <queue>
#include <vector>
#include <stack>
#include <map>
#include "user.h"
#include "DependencyGraph.h"
#include <fstream>
#include <ostream>
#include <tuple>
#include <string>
#include <stack>

using namespace std;

class spreadsheet
{
  //struct to hold a cell's name and contents
  struct cell
  {
    string cell_name;
    std::stack<string> cell_contents;
    std::stack<string> current_reverts;
  };

  struct message
  {
      string type{};
      string name{};
      string contents{};
      user* sender{};
  };

  struct edit
  {
      string name{};
      string contents{};
  };
  
  private : 
  
    // Name of the spreadsheet
    string spreadsheet_name;
        
    // Key is the name of the cell, and the value is the contents of a particular cell
    map<string, cell> non_empty_cells;
    
    // DependencyGraph g; *needs to create a dependencygraph class so that when cells are getting changed, all other cells that depend on it could be changed
    // Keeps track of all the changes that has been made to the cell
    std::stack<string> spreadsheet_history;

    std::stack<edit>* history_real;
    
    map<string, queue<string>> cell_history;    

    // Keeps track of all request messages that clients send to the server
    queue<message> message_queue;
    
    // Keeps track of all the users that are connected to the spreadsheet
    vector<user> user_list;
    //dependency graph to keep track of cells that a certain cell depend on
    DependencyGraph g;

  public :
  
    // Constructor for Spreadsheet class
    spreadsheet(string name);

    spreadsheet();
    
    // destructor
    ~spreadsheet();
    
    map<string, queue<string>> get_cell_history();

    std::stack<std::string> get_spreadsheet_history();
  
    void add_message(string new_message, int id);
    
    // Sets the name of the spreadsheet
    void set_name(string name);
    

    // Returns the name of the spreadsheet
    string get_name();
    
    // Returns the value(as opposed to the contents) of the named cell
    string get_cell_value(string name);
    
    // Returns the contents(as opposed to the values) of the named cell
    cell get_cell_contents(string name);
    
    // Returns all non-empty cells in the particular spreadsheet
    vector<string> get_names_of_all_non_empty_cells();
    
    // Passing in a bool called undo to check what the contents of the cell be set to
    bool set_contents_of_cell(string name, string content, bool undo);
    
    // Helper method that checks whether the variables belonging to a formula in name will cause a circular dependency.
    bool creates_circular_dependency(const string name, const vector<string> &vars);
    
    // This method will undo the change made to the spreadsheet
    string revert_cell(string selectedCell);
    
    string undo();

    // Adds a user to the spreadsheet
    void add_user(user* new_user);
    
    // Removes a user when they are not connected to the spreadsheet anymore
    void remove_user(int id);
    
    // Saves the non-empty cells of a spreadsheet so that a spreadsheet could be created when a user open that spreadsheet
    void save();
    
    // Server can call this method to open a spreadsheet, it will return all nonempty cells in the spreadsheet and the server can convert it to json and send it to client
    void open_spreadsheet(string file_name);
    
    // Normalize the cell contents so that they are all uppercase
    string normalize(string content);
    
    // This method will process the request sent by the clients, this method will apply the requests and the server can send the message back to the client
    bool process_messages();

    //returns the value of a cell if 
    vector<string> get_variables(string contents);

    // sends the entire spreadsheet to a socket
    void send_spreadsheet(int socket);

    // sends a disconnect message to all connected clients
    void send_disconnect(int ID);

    // sends the current selections
    void send_selections(int ID);

    //returns the dependency graph 
    DependencyGraph get_dependency_graph();

    //serializes the outgoing messages from the server to the client
    string serialize_cell_update(string messageType, string cellName, string contents);
    string serialize_cell_selected(string messageType, string cellName, int selector, string selectorName);
    string serialize_disconnected(string messageType, int user);
    string serialize_invalid_request(string messageType, string cellName, string message);
    string serialize_server_shutdown(string messageType, string message);
    //deserialize the incoming messages from the client to the server
    message deserialize_message(string input);

    vector<string> split(string str, char delimeter[]);

    queue<message> get_message_queue();

    map<string, cell> get_non_empty_cells();
};

#endif
