import socket
import random
import time
import sys
import json

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
        connectionInfo = ip_address.split(":")
        s.connect((connectionInfo[0],int(connectionInfo[1])))
        ssocket = spreadsheetSocket(s)
        return ssocket
    except Exception:
        return False
    #if this fails to connect, return false so the test stops executing.

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
    numTests = 13
    argNum = 0
    for arg in sys.argv[1:]:
        argNum+=1
        if(argNum == 1):
            test_number = int(arg)
        elif(argNum == 2):
            ip_address = arg
    if(argNum == 0):
        print(numTests)
        return 0
    else:
        testName = ""
        success = True
        if test_number == 1:
            print(5)
            success = test1(ip_address)
            testName = "Connection test"
        elif test_number == 2:
            print(20)
            success = test2(ip_address)
            testName = "Handshake test with new spreadsheet"
        elif test_number == 3:
            print(50)
            success = test3(ip_address)
            testName = "Testing selecting and editing cell with one user"
        elif test_number == 4:
            print(55)
            success = test4(ip_address)
            testName = "Testing that selection data is sent for other user present if there are two users"
        elif test_number == 5:
            print(55)
            success = test5(ip_address)
            testName = "Testing that spreadsheets are separate instances (note: runs slightly long)"
        elif test_number == 6:
            print(65)
            success = test6(ip_address)
            testName = "Basic revert test"
        elif test_number == 7:
            print(80)
            success = test7(ip_address)
            testName = "Basic undo test"
        elif test_number == 8:
            print(50)
            success = test8(ip_address)
            testName = "Basic circular dependency test"
        elif test_number == 9:
            print(110)
            success = test9(ip_address)
            testName = "More complex circular dependency test"
        elif test_number == 10:
            print(45)
            success = test10(ip_address)
            testName = "Testing that we can't edit a cell other than the one the user has selected"
        elif test_number == 11:
            print(110)
            success = test11(ip_address)
            testName = "Testing that a user can't revert to a circular dependency"
        elif test_number == 12:
            print(55)
            success = test12(ip_address)
            testName = "Test user disconnecting"
        elif test_number == 13:
            print(85)
            success = test13(ip_address)
            testName = "Test editing spreadsheet after a user disconnects"

        print(testName)
        if success:
            print("Pass")
        else:
            print("Fail")
    return 0

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method creates a new spreadsheet during the handshake.
def testHandshakeWithNewSpreadsheet(s):
    try:
        # send the client name (terminated by newline) over s.socket
        # set this name to s.name
        name = str(random.randint(-1000,1000))
        s.socket.sendall((name + "\n").encode('utf-8'))
        s.setName(name)

        #ensure that the server sends back some string that is terminated by newline
        data = receiveData(s, "\n\n", 2, 10)
        if(data is False):
            raise Exception

        # send back a spreadsheet name that doesn't currently exist on the server (terminated by newline)
        # save this spreadsheet name for later use
        # if spreadsheets exist
        if(data != "\n\n"):
            spreadsheetNames = data.split("\n")
            newSpreadsheetName = "1"
            #while spreadsheetNames contains our newSpreadsheetName, change the name so that it doesn't exist
            while(newSpreadsheetName in spreadsheetNames):
                newSpreadsheetName += "1"
            s.socket.sendall((newSpreadsheetName + "\n").encode('utf-8'))
            s.setSheet(newSpreadsheetName)
        #if spreadsheets don't currently exist on the server
        else:
            s.socket.sendall(("1\n").encode('utf-8'))
            s.setSheet("1")

        # make sure we receive expected edits (none) and some ID number, terminated by newline.
        # set this ID number to s.ID
        data = receiveData(s, "\n", 1, 10)
        if(data is False):
            raise Exception
        #set ID
        id = int(data[:-1])
        s.setID(id)
        return True

    #return false if any error occurs
    except Exception:
        return False

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method selects an existing spreadsheet during the handshake.
#the spreadsheet that goes by the given spreadsheetName is expected to have at least one edit.
def testHandshakeWithExistingSpreadsheet(s, spreadsheetName, otherUserConnected):
    try:
        # send the client name (terminated by newline) over s.socket
        # set this name to s.name
        name = str(random.randint(-1000,1000))
        s.socket.sendall((name + "\n").encode('utf-8'))
        s.setName(name)

        #receieve spreadsheet list
        data = receiveData(s, "\n\n", 2, 10)
        if(data is False):
            raise Exception

        #split all spreadsheet names
        spreadsheetNames = data.split("\n")
        #check that given spreadsheetName is in the server's list of spreadsheets
        if(spreadsheetName not in spreadsheetNames):
            return False
        #if it is in the list of spreadsheets, send the name
        else:
            s.socket.sendall((spreadsheetName + "\n").encode('utf-8'))

        #listen for cellupdates + ID
        data = ""
        id = 0
        foundID = False
        maxLoops = 15
        currentLoop = 0
        while(not foundID):
            currentLoop+=1
            if(currentLoop > maxLoops):
                return False
            data = receiveData(s, "\n", 1, 10)
            if(data is False):
                return False
            else:
                try:
                    id = int(data[:-1])
                    foundID = True
                except Exception:
                    1

        # data = receiveData(s, "\n", 1, 15)
        if(data is False):
            raise Exception
        #print(data)
        #get the ID from this data
        s.setID(id)
        return True

    #return false if any error occurs
    except Exception:
        return False

#select a given cell using the spreadsheetSocket s. spreadsheetSocket s is expected to have already completed server handshake.
#this does not test the server, and is used to assist other test functions.
def testCellSelect(s, cellName):
    # send JSON message over socket of the following form: "{requestType: "selectCell", cellName: cellName}"
    try:
        s.setCurrentCell(cellName)
        s.socket.sendall(("{\"requestType\": \"selectCell\", \"cellName\": \"" + cellName + "\"}\n").encode('utf-8'))
        return True
    # return false if there's an error sending over the socket
    except Exception:
        return False

#test selecting a given cell using socket s1. spreadsheetSockets s1 and s2 are expected to have already completed server handshake.
#ensure that s2 receives the following message after s1's cell selection:
#{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}
def testCellSelectWithTwo(s1, s2, cellName):
    # send JSON message over s1.socket of the following form: "{requestType: "selectCell", cellName: cellName}"
    try:
        s1.socket.sendall(("{\"requestType\": \"selectCell\", \"cellName\": \"" + cellName + "\"}\n").encode('utf-8'))
        s1.setCurrentCell(cellName)
    # return false if there's an error sending over the socket
    except Exception:
        return False
    #check that s2.socket has received a message as follows:
    #"{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}"
    # return true if everything processes as expected, return false if error occurs.
    data = receiveData(s2, "\n", 1, 15)
    if (data is False):
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "cellSelected" and jsonvalues["cellName"] == cellName and jsonvalues["selector"] == s1.ID and jsonvalues["selectorName"] == s1.name
    except:
        return False

#test sending a request to edit a cell on s.currentCell
#expects s to have completed server handshake.
#expects a cellUpdate message sent to s.
def testCellEdit(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall(("{\"requestType\": \"editCell\", \"cellName\": \"" + s.currentCell + "\", \"contents\": \"" + cellString + "\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    #"{messageType: "cellUpdated", cellName: s.currentCell, contents: cellString}"
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "cellUpdated" and jsonvalues["cellName"] == s.currentCell and jsonvalues["contents"] == cellString
    except Exception:
        return False

#test sending a request to edit a cell different than the one the socket currently has selected.
#expects s to have completed server handshake.
#intended to have cellName as a different value from s.currentCell - ensures that the server does not update a cell
def testCellEditIncorrectCell(s, cellString, cellName):
    #send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: cellName, contents: cellString }"
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall(("{\"requestType\": \"editCell\", cellName: \"" + cellName + "\", contents: \"" + cellString + "\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    #hang for some arbitrary time to allow for internet latency
    #check that there is no cellUpdate message sent to s
    data = receiveData(s, "\n", 1, 10)
    #receiveData should timeout as there will be no cellupdate sent, so return true if data is False.
    return data is False

# test sending a request to edit a cell on s.currentCell that will cause an error
# expects s to have completed server handshake.
# expects a requestError message sent to s.
def testCellEditError(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    try:
        s.socket.sendall(("{\"requestType\": \"editCell\", \"cellName\": \"" + s.currentCell + "\", \"contents\": \"" + cellString + "\"}\n").encode('utf-8'))

    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # "{ messageType: "requestError", cellName: s.currentCell, message: <some string> }"
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "requestError" and jsonvalues["cellName"] == s.currentCell
    except Exception:
        return False

# test sending an undo request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testUndo(s, cellName, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    try:
        s.socket.sendall(("{\"requestType\": \"undo\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "cellUpdated" and jsonvalues["cellName"] == cellName and jsonvalues["contents"] == previousValue
    except Exception:
        return False

# test sending an invalid undo request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to undo.
def testUndoError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    try:
        s.socket.sendall(("{\"requestType\": \"undo\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "requestError" and jsonvalues["cellName"] == s.currentCell
    except Exception:
        return False

# test sending a revert request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testRevert(s, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    try:
        s.socket.sendall(("{\"requestType\": \"revertCell\", \"cellName\": \"" + s.currentCell +"\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "cellUpdated" and jsonvalues["cellName"] == s.currentCell and jsonvalues["contents"] == previousValue
    except Exception:
        return False

# test sending an invalid revert request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to revert.
def testRevertError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    try:
        s.socket.sendall(("{\"requestType\": \"revertCell\", \"cellName\": \"" + s.currentCell +"\"}\n").encode('utf-8'))
    # return false if there's an error sending over the socket
    except Exception:
        return False
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }
    data = receiveData(s, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "requestError" and jsonvalues["cellName"] == s.currentCell
    except Exception:
        return False

# test disconnecting a client
# expects s1,s2 to have completed server handshake
# expects a disconnected message for s1's disconnect
def closeSpreadsheet(s1, s2):
    # close s1
    s1.socket.close()
    # ensure the following is sent to s2:
    # {messageType: "disconnected", user: s1.ID}
    data = receiveData(s2, "\n", 1, 15)
    if data is False:
        return False
    jsonvalues = json.loads(data)
    try:
        return jsonvalues["messageType"] == "disconnected" and jsonvalues["user"] == s1.ID
    except Exception:
        return False

#test connection
def test1(ip_address):
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    return True

#test handshake
def test2(ip_address):
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    return testHandshakeWithNewSpreadsheet(s1)

#test selecting and editing a cell yourself
def test3(ip_address):
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if(succeeded is False):
        return False
    if testCellSelect(s1,"A1") is False:
        return False
    return testCellEdit(s1, "abc")
    #return True

#test receiving data for someone else selecting a cell
def test4(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    #connect second socket
    s2 = connectToIP(ip_address)
    if s2 is False:
        return False
    succeeded = testHandshakeWithExistingSpreadsheet(s2, s1.sheet, True)
    if (succeeded is False):
        return False
    #test that we can see cell selection from other users
    return testCellSelectWithTwo(s1,s2,"B1")

#test that spreadsheets are separated instances
def test5(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    #connect second socket
    s2 = connectToIP(ip_address)
    if s2 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s2)
    if (succeeded is False):
        return False
    #test that we do NOT receive a cellselect update from s1
    return not testCellSelectWithTwo(s1, s2, "B2")

#test revert
def test6(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    if testCellEdit(s1, "ABC") is False:
        return False
    return testRevert(s1, "")

#test undo
def test7(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    if testCellEdit(s1, "ABC") is False:
        return False
    if testCellSelect(s1, "B2") is False:
        return False
    return testUndo(s1, "A1", "")

#test circular dependency checking
def test8(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    return testCellEditError(s1, "=A1")

#test more complex circular dependency
def test9(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "B1") is False:
        return False
    if testCellEdit(s1, "=1+1") is False:
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    if testCellEdit(s1, "=B1/2") is False:
        return False
    if testCellSelect(s1, "B1") is False:
        return False
    return testCellEditError(s1, "=A1")

#test that we get no return value from editing a cell seperate from the one we're currently assigned to
def test10(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    return testCellEditIncorrectCell(s1, "B2", "test")

#test that we can't revert to a circular dependency
def test11(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    if testCellEdit(s1, "=B1") is False:
        return False
    if testCellEdit(s1, "1") is False:
        return False
    if testCellSelect(s1, "B1") is False:
        return False
    if testCellEdit(s1, "=A1") is False:
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    return testRevertError(s1)

def test12(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    #connect second socket
    s2 = connectToIP(ip_address)
    if s2 is False:
        return False
    succeeded = testHandshakeWithExistingSpreadsheet(s2, s1.sheet, True)
    if (succeeded is False):
        return False
    return closeSpreadsheet(s1,s2)

def test13(ip_address):
    #connect first socket
    s1 = connectToIP(ip_address)
    if s1 is False:
        return False
    succeeded = testHandshakeWithNewSpreadsheet(s1)
    if (succeeded is False):
        return False
    #connect second socket
    s2 = connectToIP(ip_address)
    if s2 is False:
        return False
    succeeded = testHandshakeWithExistingSpreadsheet(s2, s1.sheet, True)
    if (succeeded is False):
        return False
    if closeSpreadsheet(s2,s1) is False:
        return False
    if testCellSelect(s1, "A1") is False:
        return False
    return testCellEdit(s1, "XYZ")

# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    main(4,"")
