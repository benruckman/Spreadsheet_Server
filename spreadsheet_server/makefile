a.out : DependencyGraph.o spreadsheet.o user.o server.o
	g++ -std=c++11 -pthread DependencyGraph.o spreadsheet.o user.o server.o
	
server.o : server.cpp
	g++ -c -std=c++11 -pthread server.cpp 
	
user.o : user.cpp user.h
	g++ -c user.cpp
	
spreadsheet.o : spreadsheet.cpp spreadsheet.h
	g++ -c spreadsheet.cpp
	
DependencyGraph.o : DependencyGraph.cpp DependencyGraph.h
	g++ -c DependencyGraph.cpp
	
clean :
	rm *.o *.out
	
