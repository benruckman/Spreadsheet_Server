/* Header for spreadsheet class
 * Authors: Jackson McKay, John Richard, Soe Min Hitke, Donnie Kubiak, Jingwen Zang, Ben Ruckman
 * Date:    4/30/21
 */

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
  // Struct to represent a cell
  struct cell
  {
    string cell_name;
    std::stack<string> cell_contents;
    std::stack<string> current_reverts;
  };

  // Struct to represent a request message
  struct message
  {
      string type{};
      string name{};
      string contents{};
      user* sender{};
  };

  // Struct to represent an edit
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
    // Keeps track of all the changes that has been made to the cell
    std::stack<edit>* history_real;
    // Keeps track of all request messages that clients send to the server
    queue<message> message_queue;
    // Keeps track of all the users that are connected to the spreadsheet
    vector<user> user_list;
    //dependency graph to keep track of cells that a certain cell depend on
    DependencyGraph g;

  public :
    // Constructors
    spreadsheet(string name);
    spreadsheet();
    ~spreadsheet();
    
    // Methods
    map<string, queue<string>> get_cell_history();
    std::stack<std::string> get_spreadsheet_history();
    void add_message(string new_message, int id);
    void set_name(string name);
    string get_name();
    string get_cell_value(string name);
    cell get_cell_contents(string name);
    vector<string> get_names_of_all_non_empty_cells();
    bool set_contents_of_cell(string name, string content, bool undo);
    bool creates_circular_dependency(const string name, const vector<string> &vars);
    string revert_cell(string selectedCell);
    string undo();
    void add_user(user* new_user);
    void remove_user(int id);
    void save();
    void open_spreadsheet(string file_name);
    string normalize(string content);
    bool process_messages();
    vector<string> get_variables(string contents);
    void send_spreadsheet(int socket);
    void send_disconnect(int ID);
    void send_selections(int ID);
    DependencyGraph get_dependency_graph();
    string serialize_cell_update(string messageType, string cellName, string contents);
    string serialize_cell_selected(string messageType, string cellName, int selector, string selectorName);
    string serialize_disconnected(string messageType, int user);
    string serialize_invalid_request(string messageType, string cellName, string message);
    string serialize_server_shutdown(string messageType, string message);
    message deserialize_message(string input);
    vector<string> split(string str, char delimeter);
    queue<message> get_message_queue();
    map<string, cell> get_non_empty_cells();
};

#endif
