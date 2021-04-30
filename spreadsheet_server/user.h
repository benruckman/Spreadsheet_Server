/* Header for User class
 * Authors: Jackson McKay, John Richard, Soe Min Hitke, Donnie Kubiak, Jingwen Zang, Ben Ruckman
 * Date:    4/30/21
 */

#ifndef USER_H
#define USER_H

#include <string>

class user
{
    private:
        int id; // A numeric identifier for this user
        int socket; // The socket id of this user
        std::string username; // A string identifier for this user
        char* buffer; // A buffer for receiving messages over a socket connection
        int size; // the size of the buffer
		std::string ssname; // the name of the spreadsheet this user is connected to 
        std::string currcell; // the current cell this user has selected
        std::string* data; // a string to hold the data that this user requests be processed
    public:
        // Constructors
		user();
        user(int id, int sock, std::string name, std::string ssname); 
        ~user(); 
    
        // Methods
        int get_id(); 
        int get_socket(); 
        std::string get_username(); 
        char* get_buffer(); 
        std::string get_ssname();
        void clear_buffer(); 
        std::string get_current();
        void set_current(std::string curr);
        std::string* get_data();
        void add_data(std::string data);
        void remove_data(int start, int length);
};

#endif
