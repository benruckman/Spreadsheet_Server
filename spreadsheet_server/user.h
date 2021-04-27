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
        int socket;
        std::string username; // A string identifier for this user
        char* buffer; // A buffer for receiving messages over a socket connection
        int size;
		std::string ssname;    
        std::string currcell;
    public:
		user();
        user(int id, int sock, std::string name, std::string ssname); // Constructor
        ~user(); // Destructor
    
        int get_id(); // Returns the user's id
        int get_socket(); // Returns the user's socket
        std::string get_username(); // Gets the username
        char* get_buffer(); // Returns the buffer
        std::string get_ssname();
        void clear_buffer(); // Sets up a new buffer
        std::string get_current();
        void set_current(std::string curr);
};

#endif
