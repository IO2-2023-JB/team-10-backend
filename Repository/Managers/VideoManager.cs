using Amazon.Runtime.Internal;
using CliWrap;
using CliWrap.Buffered;
using Entities.Data.Video;
using Entities.Models;
using Entities.Utils;
using Microsoft.Extensions.Options;

namespace Repository.Managers
{
	public class VideoManager
	{
		readonly string videoStorageLocation;
		public readonly string[] FFMpegConversionParams;

		public VideoManager(IOptions<Variables> variables)
		{
			videoStorageLocation = variables.Value.VideoStorageLocation;
			FFMpegConversionParams = variables.Value.FFMpegConversionParams;
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

		public string? GetIntroFilePath()
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;
			string path = Path.Combine(location, "intro" + ".mp4");
			if (!System.IO.File.Exists(path))
				return null;
			return path;
		}

		public (int width, int height) GetOriginalVideoResolution(string path)
		{
			//var videoInfo = new NReco.VideoInfo.FFProbe().GetMediaInfo(path).Streams;
			//return (videoInfo[0].Width, videoInfo[0].Height);

			throw new NotImplementedException();
		}

		public async Task<string?> CreateIntro((int width, int height) oryginalResolution, string introPath, string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;
			string path = Path.Combine(location, id + "_intro.mp4");
			await Cli.Wrap("ffmpeg")
				.WithArguments(
					new[]
					{
						"-i",
						introPath,
						"-vf",
						"scale=" + oryginalResolution.width + 'x' + oryginalResolution.height + ",setsar=1:1",
						path
					}
				)
				.ExecuteBufferedAsync();
			return path;
		}

		public string? GetTempFilePath(string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;
			return Path.Combine(location, id + "_temp.mp4");
		}
	}
}
