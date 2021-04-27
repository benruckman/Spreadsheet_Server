import socket
import random
import time

### Test client for WHLOL's implementation of the Jakkpot Spreadsheet protocol.
### Contains a set of tests to ensure proper implementation of the protocol.
### This client is written expecting that there are no other users on the spreadsheets being tested.
### Authors: Jingwen Zhang, Donald Kubiak

#class made to contain all relevant data alongside a given socket.
class spreadsheetSocket:
    def __init__(self, socket):
        self.socket = socket
        self.ID = -1
        self.name = ""
    def setID(self, ID):
        self.ID = ID
    def setName(self, name):
        self.name = name
    def setCurrentCell(self, currentCell):
        self.currentCell = currentCell
    def setSheet(self, sheet):
        self.sheet = sheet

#helper method that establishes a TCP connection to the given ip_address and returns the resulting spreadsheetSocket.
def connectToIP(ip_address):
    #connect to given IP using a TCP connection
    #return the spreadsheetSocket
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((ip_address, 1100))
        ssocket = spreadsheetSocket(s)
        return ssocket
    #if this fails to connect, return false so the test stops executing.
    except Exception:
        return False

#helper method for timeouts
#returns true if there has been a "timeout" (time since start time has passed the max time)
#returns false otherwise.
def timeout(startTime, maxTime):
    return time.time() - startTime > maxTime

#helper method for receiving data from a spreadsheetSocket.
#returns any data the socket receives within the given maxTime.
#if it takes longer than the given maxTime, returns false.
def receiveData(s, terminator, terminatorSize, maxTime):
    # ensure that the server sends back some string that is terminated by newline
    data = ""
    # timeout
    start = time.time()
    # while data's size is less than 2 or data hasn't been terminated by \n\n
    while (len(data) < terminatorSize or not data.endswith(terminator)):
        s.socket.settimeout(5)
        try:
            data += s.socket.recv(1024).decode('utf-8')
            #print(data[-2])
        except Exception:
            1
        # if this loop has gone on for more than a minute, timeout and return that the test failed.
        if (timeout(start, maxTime)):
            return False
    return data

#run the given number of tests on a server at the given ip address.
def main(test_number, ip_address):
    print("hello world")
    return -1

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method creates a new spreadsheet during the handshake.
def testHandshakeWithNewSpreadsheet(s):
    try:
        # send the client name (terminated by newline) over s.socket
        # set this name to s.name
        name = str(random.randint(-1000,1000))
        s.socket.sendall(name + "\n")
        s.setName(name)

        #ensure that the server sends back some string that is terminated by newline
        data = receiveData(s, "\n\n", 2, 60)
        if(data is False):
            raise Exception

        # send back a spreadsheet name that doesn't currently exist on the server (terminated by newline)
        # save this spreadsheet name for later use
        #if spreadsheets exist
        if(data != "\n\n"):
            spreadsheetNames = data.split("\n")
            newSpreadsheetName = "1"
            #while spreadsheetNames contains our newSpreadsheetName, change the name so that it doesn't exist
            while(newSpreadsheetName in spreadsheetNames):
                newSpreadsheetName += "1"
            s.socket.sendall(newSpreadsheetName + "\n")
            s.setSheet(newSpreadsheetName)
        #if spreadsheets don't currently exist on the server
        else:
            s.socket.sendall("1")
            s.setSheet("1")

        # make sure we receive expected edits (none) and some ID number, terminated by newline.
        # set this ID number to s.ID
        data = receiveData(s, "\n", 1, 60)
        if(data is False):
            raise Exception
        #set ID
        id = int(data[len(data)])
        s.setID(id)
        return True

    #return false if any error occurs
    except Exception:
        return False

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method selects an existing spreadsheet during the handshake.
#the spreadsheet that goes by the given spreadsheetName is expected to have at least one edit.
def testHandshakeWithExistingSpreadsheet(s, spreadsheetName):
    try:
        # send the client name (terminated by newline) over s.socket
        # set this name to s.name
        name = str(random.randint(-1000,1000))
        s.socket.sendall(name + "\n")
        s.setName(name)

        #receieve spreadsheet list
        data = receiveData(s, "\n\n", 2, 60)
        if(data is False):
            raise Exception

        #split all spreadsheet names
        spreadsheetNames = data.split("\n")
        #check that given spreadsheetName is in the server's list of spreadsheets
        if(spreadsheetName not in spreadsheetNames):
            return False
        #if it is in the list of spreadsheets, send the name
        else:
            s.socket.sendall(spreadsheetName + "\n")

        #listen for cellupdates + ID
        data = receiveData(s, "\n", 1, 60)
        if(data is False):
            raise Exception

#note: here we can also check the validity of the sent cellupdates - currently we are unsure of the exact form these may take so this will have
#to be updated accordingly when we talk to the TA or professor.
        #get the ID from this data
        id = int(data[data.rindex("}")+1 : len(data)-1])
        s.setID(id)

    #return false if any error occurs
    except Exception:
        return False

#select a given cell using the spreadsheetSocket s. spreadsheetSocket s is expected to have already completed server handshake.
#this does not test the server, and is used to assist other test functions.
def testCellSelect(s, cellName):
    # send JSON message over socket of the following form: "{requestType: "selectCell", cellName: cellName}"
    try:
        s.socket.sendall("{requestType: \"selectCell\", cellName: \"" + cellName + "\"}")
        return True
    # return false if there's an error sending over the socket
    except Exception:
        return False

#test selecting a given cell using socket s1. spreadsheetSockets s1 and s2 are expected to have already completed server handshake.
#ensure that s2 receives the following message after s1's cell selection:
#{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}
def testCellSelect(s1, s2, cellName):
    # send JSON message over s1.socket of the following form: "{requestType: "selectCell", cellName: cellName}"
    try:
        s1.socket.sendall("{requestType: \"selectCell\", cellName: \"" + cellName + "\"}")
        s1.setCurrentCell(cellName)
    # return false if there's an error sending over the socket
    except Exception:
        return False
    #check that s2.socket has received a message as follows:
    #"{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}"
    # return true if everything processes as expected, return false if error occurs.
    data = receiveData(s2, "}", 1, 60)
    if (data is False):
        return False

    return "messageType: \"selected\", cellName: \"" + cellName + "\", selector: " + s1.ID + ", selectorName: \"" + s1.name + "\"" in data

#test sending a request to edit a cell on s.currentCell
#expects s to have completed server handshake.
#expects a cellUpdate message sent to s.
def testCellEdit(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall("{requestType: \"editCell\", cellName: \"" + s.currentCell + "\", contents: \"" + cellString + "\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    #"{messageType: "cellUpdated", cellName: s.currentCell, contents: cellString}"
    data = receiveData(s, "}", 1, 60)
    return "messageType: \"cellUpdated\", cellName: \"" + s.currentCell + "\", contents: \"" + cellString + ", selectorName: \"" + s.name + "\"" in data

#test sending a request to edit a cell different than the one the socket currently has selected.
#expects s to have completed server handshake.
#intended to have cellName as a different value from s.currentCell - ensures that the server does not update a cell
def testCellEdit(s, cellString, cellName):
    #send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: cellName, contents: cellString }"
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall("{requestType: \"editCell\", cellName: \"" + cellName + "\", contents: \"" + cellString + "\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    #hang for some arbitrary time to allow for internet latency
    #check that there is no cellUpdate message sent to s
    data = receiveData(s, "}", 1, 30)
    #receiveData should timeout as there will be no cellupdate sent, so return true if data is False.
    return data is False

# test sending a request to edit a cell on s.currentCell that will cause an error
# expects s to have completed server handshake.
# expects a requestError message sent to s.
def testCellEditError(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall("{requestType: \"editCell\", cellName: \"" + s.currentCell + "\", contents: \"" + cellString + "\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # "{ messageType: "requestError", cellName: s.currentCell, message: <some string> }"
    data = receiveData(s, "}", 1, 60)
    return ("messageType: \"requestError\", cellName: " + s.currentCell + ", message:") in data

# test sending an undo request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testUndo(s, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    try:
        s.socket.sendall("{requestType: \"undo\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }
    data = receiveData(s, "}", 1, 60)
    return "messageType: \"cellUpdated\", cellName: " + s.currentCell + ", contents: \"" + previousValue + "\"" in data

# test sending an invalid undo request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to undo.
def testUndoError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    try:
        s.socket.sendall("{requestType: \"undo\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }
    data = receiveData(s, "}", 1, 60)
    return ("messageType: \"requestError\", cellName: " + s.currentCell + ", message:") in data

# test sending a revert request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testRevert(s, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    try:
        s.socket.sendall("{requestType: \"revertCell\", \"cellName\": \"" + s.currentCell +"\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }
    data = receiveData(s, "}", 1, 60)
    return "messageType: \"cellUpdated\", cellName: " + s.currentCell + ", contents: \"" + previousValue + "\"" in data

# test sending an invalid revert request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to revert.
def testRevertError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    try:
        s.socket.sendall("{requestType: \"revertCell\", \"cellName\": \"" + s.currentCell +"\"}")
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }
    data = receiveData(s, "}", 1, 60)
    return ("messageType: \"requestError\", cellName: " + s.currentCell + ", message:") in data

# test disconnecting a client
# expects s1,s2 to have completed server handshake
# expects a disconnected message for s1's disconnect
def closeSpreadsheet(s1, s2):
    # close s1
    s1.socket.close()
    # ensure the following is sent to s2:
    # {messageType: "disconnected", user: s1.ID}
    data = receiveData(s2, "}", 1, 60)
    return "messageType: \"disconnected\", user: " + s1.ID in data

# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    main(0,0)
