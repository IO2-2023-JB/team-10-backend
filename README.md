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

Fix formatting:

    dotnet csharpier .

Check formatting:

    dotnet csharpier --check .

### Config file

The configuration file _.csharpierrc.json_ is located in the solution folder.

## Release

WebAPI runs as the io2_api_production.service service on our VM. The production build is stored in the `~/production/backend` directory.

### Publishing

This is done automatically after every merge to master by the `deploy_on_vm.yml` Github Action. Here are the instruction to publish a new version manually (run in the root folder of this repository):

    dotnet publish -c Release -o ~/production/backend
    sudo systemctl restart io2_api_production.service

### Service

To add the service on another machine, create the file `/etc/systemd/system/io2_api_production.service` with the following contents:

    [Unit]
    Description=mojeWidelo webApi

    [Service]
    WorkingDirectory=/home/ubuntu/production/backend
    ExecStart=dotnet /home/ubuntu/production/backend/MojeWidelo_WebApi.dll
    Restart=always
    RestartSec=10
    SyslogIdentifier=mojeWidelo_WebApi_Production
    Environment=ASPNETCORE_ENVIRONMENT=Production

    [Install]
    WantedBy=multi-user.target

Remember to adjust the path of the build. Control the service using the command

    sudo systemctl X io2_api_production.service

where `X` is one of

- `start`
- `stop`
- `restart`
- `status`

### Search API service

To add the service on another machine, create the file `/etc/systemd/system/io2_api_search.service` with the following contents:

    [Unit]
    Description=mojeWidelo python api

    [Service]
    User=ubuntu
    WorkingDirectory=/home/ubuntu/production/search
    ExecStart=/home/ubuntu/.local/bin/poetry run start
    Restart=always
    RestartSec=10
    SyslogIdentifier=mojeWidelo_python_api_Production

    [Install]
    WantedBy=multi-user.target

Control the service as in the # Service section (despite the fact that in this case service is named io2_api_search.service).

## Video storage

The location of video storage should be set in the `appsettings.json` file (`appsettings.Production.json` for production).

## Video processing

Software called FFmpeg is used to convert uploaded video files to desired format. It is necessary to have FFmpeg installed in order to use POST /video/{id} endpoint.

### FFmpeg instalation (for Windows users)

Installing FFmpeg is simply matter of following instructions from link below:

https://www.wikihow.com/Install-FFmpeg-on-Windows

### FFmpeg instalation (for Linux users)

    sudo apt install ffmpeg
