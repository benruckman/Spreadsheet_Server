a.out : Spreadsheet.o user.o server.o
	g++ -std=c++11 -pthread Spreadsheet.o user.o server.o
	
server.o : server.cpp
	g++ -c -std=c++11 -pthread server.cpp
	
user.o : user.cpp user.h
	g++ -c user.cpp
	
Spreadsheet.o : Spreadsheet.cpp Spreadsheet.h
	g++ -c Spreadsheet.cpp
	
clean :
	rm *.o *.out
	
