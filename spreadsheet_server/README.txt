build command: docker image build --tag server_wehatelol . 

run server command: docker run --mount type=bind,source=/<some folder>,target=/spreadsheet_data -p 1100:1100 server_wehatelol /spreadsheet_server

ctrl+c to close.
