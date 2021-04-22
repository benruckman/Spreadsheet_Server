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

class Spreadsheet
{
  //struct to hold a cell's name and contents
  struct Cell
  {
    string cellName;
    string cellContents;
  };
  
 public:
 
  //default constructor for Spreadsheet class
  Spreadsheet();
  
  //this constructor will be used to open an existing spreadsheet or create one if it doesn't exist
  //  Spreadsheet(string fileName);
  //destructor
  ~Spreadsheet();
  
  //name of the spreadsheet
  string SpreadSheetName;
  
  // returns the name of the spreadsheet
  string get_name();
  
  //returns the value(as opposed to the contents) of the named cell
  string GetCellValue(string name);
  
  //returns the contents(as opposed to the values) of the named cell
  string GetCellContents(string name);
  
  //returns all non-empty cells in the particular spreadsheet
  vector<string> GetNamesOfAllNonEmptyCells();
  
  //passing in a bool called undo to check what the contents of the cell be set to
  bool SetContentsOfCell(string name, string content, bool undo);
  
  //this method will undo the change made to the spreadsheet
  string undo_spreadSheet();
  
  //adds a user to the spreadsheet
  void add_user(user newUser);
  
  //removes a user when they are not connected to the spreadsheet anymore
  void remove_user(int id);
  
  //saves the non-empty cells of a spreadsheet so that a spreadsheet could be created when a user open that spreadsheet
  void Save();
  
  //key is the name of the cell, and the value is the contents of a particular cell
  map<string, string> NonEmptyCells;
  
  //DependencyGraph g; *needs to create a dependencygraph class so that when cells are getting changed, all other cells that depend on it could be changed
  //keeps track of all the changes that has been made to the cell
  queue<Cell> CellHistory;
  
  //keeps track of all request messages that clients send to the server
  queue<string> MessageQueue;
  
  //keeps track of all the users that are connected to the spreadsheet
  vector<user> UserList;
  
  //server can call this method to open a spreadsheet, it will return all nonempty cells in the spreadsheet and the server can convert it to json and send it to client
  map<string, string> open_spreadsheet(string fileName);
  
  //normalize the cell contents so that they are all uppercase
  string normalize(string content);
  
  //this method will process the request sent by the clients, this method will apply the requests and the server can send the message back to the client
  bool process_messages();
  
 private:
  //could have methods to check circular dependency
};

#endif
