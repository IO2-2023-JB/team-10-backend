#!/bin/bash

usage()
{
   echo ""
   echo "Usage: $0 [ -n user_friendly_backup_name -c collection_name]"
   exit 1 
}

while getopts ":n:c:" opt; do
    case $opt in
        n) name="_$OPTARG" ;;
        c) collection_name=$OPTARG ;;
        \?) usage ;;
        :) usage ;;
    esac
done

database_name=mojeWideloDb_Production
backup_name="$HOME"/io2/mongo_backups/$(date +"%Y_%m_%d_%H_%M_%S")"$name"
video_storage="$HOME"/production/video-storage

mongodump --out="$backup_name" --db="$database_name" --collection="$collection_name"

echo ""
echo "Copying video storage..."
cp -R "$video_storage" "$backup_name"
echo "Video storage successfully copied."

echo ""
echo "Backup successfully created in: $backup_name"