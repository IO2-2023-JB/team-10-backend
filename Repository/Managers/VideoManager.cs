using CliWrap;
using CliWrap.Buffered;
using Entities.Data.Video;
using Entities.Models;
using Entities.Utils;
using Microsoft.Extensions.Options;
using System.Text;

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

		public async Task<string?> CreateIntro(
			string originalResolution,
			string originalFPS,
			string introPath,
			string id
		)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			string pathTemp = Path.Combine(location, id + "_intro_temp.mp4");
			string path = Path.Combine(location, id + "_intro.mp4");

			await Cli.Wrap("ffmpeg")
				.WithArguments(
					new[] { "-i", introPath, "-vf", "scale=" + originalResolution, "-r", originalFPS, pathTemp }
				)
				.ExecuteBufferedAsync();

			var arguments = new List<string>(new[] { "-i", pathTemp });
			arguments.AddRange(FFMpegConversionParams);
			arguments.Add(path);
			await Cli.Wrap("ffmpeg").WithArguments(arguments).ExecuteBufferedAsync();

			System.IO.File.Delete(pathTemp);
			if (System.IO.File.Exists(pathTemp))
				throw new Exception("Unable to delete temp intro video file!");

			return path;
		}

		public string? GetTempFilePath(string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;
			return Path.Combine(location, id + "_temp.mp4");
		}

		public async Task ConcatVideo(string id, string introTempPath, string tempPath, string newPath)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				throw new Exception("Unable to get storage directory!");

			string txt = Path.Combine(location, id + "_elements.txt");

			using (StreamWriter sw = File.CreateText(txt))
			{
				sw.WriteLine("file '" + introTempPath + "'");
				sw.WriteLine("file '" + tempPath + "'");
			}

			await Cli.Wrap("ffmpeg")
				.WithArguments(new[] { "-f", "concat", "-safe", "0", "-i", txt, "-c", "copy", newPath })
				.ExecuteBufferedAsync();

			System.IO.File.Delete(txt);
			if (System.IO.File.Exists(txt))
				throw new Exception("Unable to delete elements file!");
		}
	}
}
