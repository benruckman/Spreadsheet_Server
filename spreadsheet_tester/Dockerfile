FROM ubuntu

# Set a default shell.

#SHELL ["/bin/bash", "-c"]

# The timezone library stops the docker build and waits for user input to
# select a timezone. This pauses/breaks the build. To get around this, 
# set up timezone environment variables prior to installing that library. 
# This Docker code does that. Composited from two sources:
# https://rtfm.co.ua/en/docker-configure-tzdata-and-timezone-during-build/
# https://serverfault.com/questions/949991/how-to-install-tzdata-on-a-ubuntu-docker-image

ARG DEBIAN_FRONTEND=noninteractive
ENV TZ="America/Salt Lake City"

# Install a handful of useful libraries.  Note that this is generally against
# best practices -- containers should be lean, and not include 'things you
# might use'.  In our case, we're using the containers for development, so
# this may be reasonable temporarily.

COPY . .
   
RUN apt-get -y update && apt-get -y install python3

RUN chmod 777 spreadsheet_tester
