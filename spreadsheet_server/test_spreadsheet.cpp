#include "spreadsheet.h"
#include <iostream>

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
  spreadsheet t("messagesTest");
  tuple<string, string, string> test = t.deserialize_message("{requestType: \"editCell\", cellName: A1, contents: 4}");
  std::cout<<get<0>(test)<<" " <<get<1>(test)<<" "<<get<2>(test)<<std::endl;
  string test1 = t.serialize_cell_update("cellUpdated", "A4", "10");
  std::cout<<test1<<std::endl;
  string test2 = t.serialize_cell_selected("cellSelected", "A7", 1, "User1");
  std::cout<<test2<<std::endl;
  string test3 = t.serialize_disconnected("disconnected", 10);
  std::cout<<test3<<std::endl;
  string test4 = t.serialize_invalid_request("requestError", "A4", "Request Error!");
  std::cout<<test4<<std::endl;
  string test5 = t.serialize_server_shutdown("serverError", "Server Shutdown!");
  std::cout<<test5<<std::endl;
  t.add_message("{requestType: \"editCell\", cellName: A2, contents: 50}");
  tuple<string, string, string> firstMessage = t.get_message_queue().front();
  std::cout<<get<0>(firstMessage)<<" " <<get<1>(firstMessage)<<" "<<get<2>(firstMessage)<<std::endl;
  return 0;    
}
