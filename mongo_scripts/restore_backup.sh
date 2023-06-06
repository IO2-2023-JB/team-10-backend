#!/bin/bash

usage()
{
   echo ""
   echo "Usage: $0 -d backup_directory"
   exit 1 
}

while getopts "d:" opt; do
    case $opt in
        d) dir_name="$OPTARG" ;;
        \?) usage ;;
        :) usage ;;
    esac
done

if [ -z "$dir_name" ]; then
    usage
fi

echo $dir_name

storage=video-storage
production_video_storage="$HOME"/production/"$storage"

mongorestore "$dir_name"

echo ""
echo "Copying video storage..."
rm -rf "$production_video_storage"
cp -R "$dir_name/$storage" "$production_video_storage"
echo "Video storage successfully copied."

echo ""
echo "Backup successfully restored."