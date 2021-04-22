/*
 Definitions for the spreadsheet class
 */
#include <iostream>
#include "Spreadsheet.h"

using namespace std;

Spreadsheet::Spreadsheet()
{

}

Spreadsheet::~Spreadsheet()
{

}

string Spreadsheet::get_name()
{
  return SpreadSheetName;
}

string Spreadsheet::GetCellContents(string name)
{
  //googled stackoverflow for how to iterate through a map
  for(map<string, string>::iterator it = NonEmptyCells.begin(); it != NonEmptyCells.end(); it++)
  {
    //it->first is the key of the map
    if(it->first == name)
      return NonEmptyCells[name];
  }
  return "";
}

vector<string> Spreadsheet::GetNamesOfAllNonEmptyCells()
{
  vector<string> cells;
  //this is the way to iterate through a map
  for(map<string, string>::iterator it = NonEmptyCells.begin(); it != NonEmptyCells.end(); it++)
  {
    cells.push_back(it->first);
  }
  return cells;
}
//this method must check for circular dependencies
bool Spreadsheet::SetContentsOfCell(string name, string content, bool undo)
{
  //used to keep the previous contents of a cell
  string previousContents = "";
  if(undo == false && NonEmptyCells.size() != 0)
  {
    for(map<string, string>::iterator it = NonEmptyCells.begin(); it != NonEmptyCells.end(); it++)
    {
      //if the key in the map is equal to name of the cell
      if(it->first == name)
      {
	//it->second is the value associated with the key
	previousContents = it->second;
	//needs to replace the dependents from the dependency graph
      }
    }
    NonEmptyCells[name] = content;
    //needs to check if the content is a formula, if it is, then we would need a way to return variable in the formula
    //then check for circular dependencies and replace dependents and add dependencies 
    // if(content[0] == "=")
  }
}

void Spreadsheet::add_user(user newUser)
{
  UserList.push_back(newUser);
}

void Spreadsheet::remove_user(int id)
{
  for(vector<user>::iterator it = UserList.begin(); it != UserList.end(); it++)
  {
    if((* it).get_id() == id)
    {
      UserList.erase(it);
    }
  }
}

string Spreadsheet::undo_spreadSheet()
{
  Cell previousCell = CellHistory.front();
  CellHistory.pop();
  string cellName = previousCell.cellName;
  string cellContent = previousCell.cellContents;
  SetContentsOfCell(cellName, cellContent, true);
  string newChange = cellName + " : " + cellContent;
  return newChange;
}

void Spreadsheet::Save()
{
  ofstream f;
  string fileName = SpreadSheetName + ".txt";
  //needs to pass in a c string to open the file
  f.open(fileName.c_str());
  //iterate through the nonempty cell and write them to a text file
  for(map<string, string>::iterator it = NonEmptyCells.begin(); it != NonEmptyCells.end(); it++)
  { 
    //it->first is the cell name in the map, it->second is the contents of the cell in the map
    string cells = it->first + " : " + it->second + "\n";
    f<<cells;
  }
  f.close();
}

map<string, string> Spreadsheet::open_spreadsheet(string fileName)
{
  string sheetName = fileName + ".txt";
  ifstream f;
  string cellName = "";
  string cellContents;
  f.open(sheetName.c_str());
  while(f>>cellName>>cellContents)
  {
    SetContentsOfCell(cellName, cellContents, true);
  }
  f.close();
  return NonEmptyCells;
}

string Spreadsheet::normalize(string cellContents)
{
  string normalizedContents;
  for(int i = 0; i < cellContents.size(); i++)
  {
    normalizedContents += toupper(cellContents[i]);
  }
  return normalizedContents;
}

bool Spreadsheet::process_messages()
{
  return true;
}
