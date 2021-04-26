#include "spreadsheet.h"


/*
 * Tests the constructor
 * Returns true if the test is successful and false if the test is unsuccessful.
 */
bool test_constructor()
{
  spreadsheet s("test");
  
  return s.get_name() == "test";
}


/*
 * Tests set_name and get_name.
 * Returns true if the test is successful and false if the test is unsuccessful.
 */
bool test_set_name()
{
  bool result = true;
  
  spreadsheet s("ONE");
  result = result && s.get_name() == "ONE";
  
  s.set_name("TWO");
  result = result && s.get_name() == "TWO";
  
  s.set_name("THREE");
  result = result && s.get_name() == "THREE";
  
  return result;
}


int main()
{
  if(!test_constructor())
  {
    std::cout << "test_constructor: FAILED" << std::endl;
    return 1;;
  }
  else
    std::cout << "test_constructor: PASSED" << std::endl;
    
  if(!test_set_name())
  {
    std::cout << "test_set_name: FAILED" << std::endl;
    return 1;
  }
  else
    std::cout << "test_set_name: PASSED" << std::endl;
  
  return 0;    
}
