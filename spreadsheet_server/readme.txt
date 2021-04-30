Team name: we_hate_league

Members:
Ben Ruckman
Donnie Kubiak
Jackson McKay
Jingwen Zhang
John Richard
Soe Min Htike

To build the docker container for the server, use this command:
    docker image build --tag server_we_hate_lol . 

After building, run the server with this command:
    docker run --mount type=bind,source=/<some folder>,target=/spreadsheet_data -p 1100:1100 server_we_hate_lol /spreadsheet_server

Ctrl+c will close a running server.
