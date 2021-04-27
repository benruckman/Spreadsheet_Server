#ifndef DEPENDENCYGRAPH_H
#define DEPENDENCYGRAPHT_H

#include <iostream>
#include <map>
#include <set>

using namespace std;

class DependencyGraph{
  

 public:
   DependencyGraph(); 
   ~DependencyGraph();
   int size() const;
   int getDependeesNumber(const string &s) const;
   bool hasDependents(const string &s) const;
   bool hasDependees(const string &s) const;
   set<string> getDependees(const string &s) const;
   set<string> getDependents(const string &s) const;
   void addDependency(const string &s, const string &t);
   void removeDependency(const string &s, const string &t);
   void replaceDependents(const string &s, const set<string> newDependents);
   void replaceDependees(const string &s, const set<string> newDependees);



 private:
  map<string, set<string> > dependents;
  map<string, set<string> > dependees;
  int p_size;
};

#endif
