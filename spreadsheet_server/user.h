#ifndef USER_H
#define USER_H

#include <string>

/*
 * TODO: Fill out documentation
 */
class user
{
    private:
        int id; // A numeric identifier for this user
        std::string username; // A string identifier for this user
        char* buffer; // A buffer for receiving messages over a socket connection
    
    public:
        user(int buffer_size); // Constructor
        ~user(); // Destructor
    
        int get_id(); // Returns the user's id
        void set_id(int new_id); // Sets the user's id
        
        void set_username(std::string new_username); // Sets the username
        std::string get_username(); // Gets the username
        
        char* get_buffer(); // Returns the buffer
};

#endif