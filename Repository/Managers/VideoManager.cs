﻿using Entities.Data.Video;
using Entities.Models;
using Entities.Utils;
using Microsoft.Extensions.Options;

namespace Repository.Managers
{
	public class VideoManager
	{
		readonly string videoStorageLocation;

		public VideoManager(IOptions<Variables> variables)
		{
			videoStorageLocation = variables.Value.VideoStorageLocation;
		}

		public string? CreateNewPath(string id, string fileName)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			string extension = Path.GetExtension(fileName);
			return Path.Combine(location, id + "_original" + extension);
		}

		public string? GetStorageDirectory()
		{
			string location = Path.GetFullPath(videoStorageLocation);
			Directory.CreateDirectory(location);

			if (string.IsNullOrEmpty(location))
				return null;

			return location;
		}

		public string? GetReadyFilePath(string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			return Path.Combine(location, id + ".mp4");
		}

		public void AddAuthorNickname(IEnumerable<VideoMetadataDto> videos, IEnumerable<User> users)
		{
			foreach (var video in videos)
			{
				var videoAuthor = users.Where(user => user.Id == video.AuthorId).SingleOrDefault();
				if (videoAuthor != null)
					video.AuthorNickname = videoAuthor.Nickname;
				else
					throw new Exception("Nie znaleziono użytkownika");
			}
		}

		public void AddThumbnailUri(Uri location, IEnumerable<VideoMetadataDto> videos)
		{
			foreach (var video in videos)
			{
				if (video.Thumbnail != null)
				{
					video.Thumbnail = location.AbsoluteUri + video.Thumbnail;
				}
			}
		}
	}
}
