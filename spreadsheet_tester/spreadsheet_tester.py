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

#helper method that establishes a TCP connection to the given ip_address and returns the resulting socket.
def connectToIP(ip_address):
    #connect to given IP using a TCP connection
    #return the spreadsheetSocket

#run the given number of tests on a server at the given ip address.
def main(test_number, ip_address):
    #attempt connection to given ip_address

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method creates a new spreadsheet during the handshake.
def testHandshakeWithNewSpreadsheet(s):
    #send the client name (terminated by newline) over s.socket
        #set this name to s.name
    #ensure that the server sends back some string that is terminated by newline
    #send back a spreadsheet name that doesn't currently exist on the server (terminated by newline)
        #save this spreadsheet name for later use
    #make sure we receive expected edits (none) and some ID number, terminated by newline.
        #set this ID number to s.ID
    #return true if everything processes as expected, return false if error occurs.

#test a correct handshake over spreadsheetSocket s, ensure that the proper data is sent back by server
#this method selects an existing spreadsheet during the handshake.
#the spreadsheet that goes by the given spreadsheetName is expected to have at least one edit.
def testHandshakeWithExistingSpreadsheet(s, spreadsheetName):
    #send the client name (terminated by newline) over s.socket
        #set this name to s.name
    #ensure that the server sends back some string that is terminated by newline and that the string contains spreadsheetName, followed by newline
    #send back spreadsheetName (terminated by newline)
    #make sure we receive expected edits (at least one) and some ID number, terminated by newline.
        #set this ID number to s.ID
    #return true if everything processes as expected, return false if error occurs.

#select a given cell using the spreadsheetSocket s. spreadsheetSocket s is expected to have already completed server handshake.
#this does not test the server, and is used to assist other test functions.
def testCellSelect(s, cellName):
    #send JSON message over socket of the following form: "{requestType: "selectCell", cellName: cellName}"

#test selecting a given cell using socket s1. spreadsheetSockets s1 and s2 are expected to have already completed server handshake.
#ensure that s2 receives the following message after s1's cell selection:
#{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}
def testCellSelect(s1, s2, cellName):
    #send JSON message over s1.socket of the following form: "{requestType: "selectCell", cellName: cellName}"
    #hang for some arbitrary time to allow for internet latency
    #check that s2.socket has received a message as follows:
    #"{messageType: "selected", cellName: cellName, selector: s1.ID, selectorName: s1.name}"
    # return true if everything processes as expected, return false if error occurs.

#test sending a request to edit a cell on s.currentCell
#expects s to have completed server handshake.
#expects a cellUpdate message sent to s.
def testCellEdit(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    #"{messageType: "cellUpdated", cellName: s.currentCell, contents: cellString}"

#test sending a request to edit a cell different than the one the socket currently has selected.
#expects s to have completed server handshake.
#intended to have cellName as a different value from s.currentCell - ensures that the server does not update a cell
def testCellEdit(s, cellString, cellName):
    #send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: cellName, contents: cellString }"
    #hang for some arbitrary time to allow for internet latency
    #check that there is no cellUpdate message sent to s

# test sending a request to edit a cell on s.currentCell that will cause an error
# expects s to have completed server handshake.
# expects a requestError message sent to s.
def testCellEditError(s, cellString):
    # send JSON message over s.socket of the following form : "{ requestType: "editCell", cellName: s.currentCell, contents: cellString }"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    # "{ messageType: "requestError", cellName: s.currentCell, message: <some string> }"

# test sending an undo request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testUndo(s, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }

# test sending an invalid undo request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to undo.
def testUndoError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "undo"}"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }

# test sending a revert request
# expects s to have completed server handshake and for the currently selected cell to have a backlog of changes.
# expects previousValue to be the value entered immediately before the most recent value entered into this cell.
def testRevert(s, previousValue):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    # { messageType: "cellUpdated", cellName: s.currentCell, contents: previousValue }

# test sending an invalid revert request
# expects s to have completed server handshake and for the currently selected cell to NOT have a backlog of changes.
# expects error to occur since there is nothing to revert.
def testRevertError(s):
    # send JSON message over s.socket of the following form : "{"requestType": "revertCell", "cellName": s.currentCell}"
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s:
    # { messageType: "requestError", cellName: s.currentCell, message: <some string> }

# test disconnecting a client
# expects s1,s2 to have completed server handshake
# expects a disconnected message for s1's disconnect
def closeSpreadsheet(s1, s2):
    # close s1
    # hang for some arbitrary time to allow for internet latency
    # ensure the following is sent to s2:
    # {messageType: "disconnected", user: s1.ID}

# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    main()
