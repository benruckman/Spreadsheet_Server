/*
 Header file for spreadsheet class declarations
 */
//needs to create DependencyGraph class to use its helper functions

#ifndef SPREADSHEET_H
#define SPREADSHEET_H

#include <iostream>
#include <queue>
#include <vector>
#include <stack>
#include <map>
#include "user.h"
#include <fstream>
#include <ostream>

using namespace std;

class spreadsheet
{
  //struct to hold a cell's name and contents
  struct cell
  {
    string cell_name;
    string cell_contents;
  };
  
  private : 
  
    // Name of the spreadsheet
    string spreadsheet_name;
        
    // Key is the name of the cell, and the value is the contents of a particular cell
    map<string, string> non_empty_cells;
    
    // DependencyGraph g; *needs to create a dependencygraph class so that when cells are getting changed, all other cells that depend on it could be changed
    // Keeps track of all the changes that has been made to the cell
    queue<cell> cell_history;
    
    // Keeps track of all request messages that clients send to the server
    queue<string> message_queue;
    
    // Keeps track of all the users that are connected to the spreadsheet
    vector<user> user_list;
    
  public :
  
    // Constructor for Spreadsheet class
    spreadsheet(string name);
    
    // destructor
    ~spreadsheet();
    
    // Adds a message to the message queue
    void add_message(string new_message);
    
    // Sets the name of the spreadsheet
    void set_name(string name);
    
    // Returns the name of the spreadsheet
    string get_name();
    
    // Returns the value(as opposed to the contents) of the named cell
    string get_cell_value(string name);
    
    // Returns the contents(as opposed to the values) of the named cell
    string get_cell_contents(string name);
    
    // Returns all non-empty cells in the particular spreadsheet
    vector<string> get_names_of_all_non_empty_cells();
    
    // Passing in a bool called undo to check what the contents of the cell be set to
    bool set_contents_of_cell(string name, string content, bool undo);
    
    // This method will undo the change made to the spreadsheet
    string undo_spreadsheet();
    
    // Adds a user to the spreadsheet
    void add_user(user new_user);
    
    // Removes a user when they are not connected to the spreadsheet anymore
    void remove_user(int id);
    
    // Saves the non-empty cells of a spreadsheet so that a spreadsheet could be created when a user open that spreadsheet
    void save();
    
    // Server can call this method to open a spreadsheet, it will return all nonempty cells in the spreadsheet and the server can convert it to json and send it to client
    map<string, string> open_spreadsheet(string file_name);
    
    // Normalize the cell contents so that they are all uppercase
    string normalize(string content);
    
    // This method will process the request sent by the clients, this method will apply the requests and the server can send the message back to the client
    bool process_messages();
};

#endif
