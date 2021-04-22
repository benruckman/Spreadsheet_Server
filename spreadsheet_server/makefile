a.out : server.o user.o
	g++ -std=c++11 -pthread server.o user.o
	
server.o : server.cpp
	g++ -c -std=c++11 -pthread server.cpp
	
user.o : user.cpp user.h
	g++ -c user.cpp
	
clean :
	rm *.o *.out
	
