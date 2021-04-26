#include "DependencyGraph.h"
#include <iostream>
#include <map>
#include <set>

using namespace std;

DependencyGraph:: DependencyGraph()
{
  map<string, set<string> > dependents;
  map<string, set<string> > dependees;
  int p_size = 0;
} 

DependencyGraph::~DependencyGraph()
{
  dependents.clear();
  dependees.clear();
}

int DependencyGraph::size() const
{
  return p_size; 
}

int DependencyGraph::getDependeesNumber(const string &s) const
{
  auto itr = dependees.find(s);
  if(itr != dependees.end()) return itr->first.size();
  return 0;
}

bool DependencyGraph::hasDependents(const string &s) const
{
  auto itr = dependents.find(s);
  return itr != dependents.end(); 
}

bool DependencyGraph::hasDependees(const string &s) const
{
  auto itr = dependees.find(s);
  return itr != dependees.end(); 
}

set<string> DependencyGraph::getDependees(const string &s) const
{
  set<string> dependeeList;
  auto itr = dependees.find(s);
  if(itr != dependees.end())
  {
    dependeeList = itr->second; 
  }
  return dependeeList; 
}

set<string> DependencyGraph::getDependents(const string &s) const
{
  set<string> dependentList;
  auto itr = dependents.find(s);
  if(itr != dependents.end())
  {
    dependentList = itr->second;
  }
  return dependentList;
}

void DependencyGraph::addDependency(const string &s, const string &t)
{
    bool pairAdded;
    
    if(hasDependents(s))
    {
        set<string> dependentList_s = getDependents(s);
        if(dependentList_s.find(t) == dependentList_s.end())
        {
            
            dependentList_s.insert(t);
            pairAdded = 1;
            
        }
        else
        {
            set<string> st;
            st.insert(t);
            dependents.insert({s, st});
            pairAdded = 1;
        }
        
    }
    
    if(hasDependees(t))
    {
        set<string> dependees_t = getDependees(t);
        if(dependees_t.find(s) == dependees_t.end()) 
        {
            dependees_t.insert(s);
            pairAdded = 1;
        }
        
        else
        {
            set<string> st;
            st.insert(s);
            dependees.insert({t, st});
            pairAdded = 1;
        }
        
    }
    
    if(pairAdded) p_size++;
}
void DependencyGraph::removeDependency(const string &s, const string &t){
}
void DependencyGraph::replaceDependents(const string &s, const set<string> newDependents){
}
void DependencyGraph::replaceDependees(const string &s, const set<string> newDependees){
}




  

