Team name: we_hate_league

Members:
Ben Ruckman
Donnie Kubiak
Jackson McKay
Jingwen Zhang
John Richard
Soe Min Htike

Server:
    To build the docker container for the server, use this command:
        docker image build --tag server_we_hate_league .

    After building, run the server with this command (make sure to name a directoy to mount):
        docker run --mount type=bind,source=/[insert some directory here],target=/spreadsheet_data -p 1100:1100 server_we_hate_league /spreadsheet_server

    Ctrl+c will close a running server.

Tester:
    To build the docker container for the server, use this command:
        docker image build --tag tester_we_hate_league .
        
        docker run tester_we_hate_league /spreadsheet_tester [arguments]
