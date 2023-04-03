# MojeWideło Team 10 Back-End

## Overview

This repository contains the backend of the MojeWideło application - a video streaming platform implemented during the software engineering course at the Faculty of Mathematics and Information Science of the Warsaw University of Technology.

## Code formatting

This project uses [Csharpier](https://csharpier.com/) for code formatting. Please always format your code before commiting.

### Installation

    dotnet tool install -g csharpier

### Format on save

Install extension for your IDE and follow steps to enable format on save [here](https://csharpier.com/docs/Editors).

### Formatting in CLI

Before first use:

    dotnet tool restore

Check formatting:

    dotnet csharpier .

Fix formatting:

    dotnet csharpier --check .

### Config file

The configuration file _.csharpierrc.json_ is located in the solution folder.

## Release

WebAPI runs as service io2_api.service on our VM.

### Manual rebuild

    cd ~/io2/team-10-backend
    git pull
    sudo dotnet publish -c Release -o /var/www/io2/backend/
    sudo systemctl restart io2_api.service

### Config file for io2_api.service:

    /etc/systemd/system/io2_api.service
	
## Video storage

For video upload/download to function properly, user must first configure environment variable called MojeWideloStorage, which shall be an absolute path to the directory that is designated to be storage place for video

### Command Line code (for Windows users)

	setx MojeWideloStorage {path} /m
	(command must be executed with administrator privileges)
	
### Bash command 

	soon
