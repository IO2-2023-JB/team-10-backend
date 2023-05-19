using CliWrap;
using CliWrap.Buffered;
using Contracts;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Models;
using Entities.Utils;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Repository.Managers;
using System.Text.RegularExpressions;

namespace Repository
{
	public class VideoRepository : RepositoryBase<VideoMetadata>, IVideoRepository
	{
		readonly VideoManager videoManager;

		public VideoRepository(IDatabaseSettings databaseSettings, VideoManager videoManager)
			: base(databaseSettings, databaseSettings.VideoCollectionName)
		{
			this.videoManager = videoManager;
		}

		public async Task ChangeVideoProcessingProgress(string id, ProcessingProgress progress)
		{
			VideoMetadata video = await GetById(id);
			ProcessingProgress pastProgress = video.ProcessingProgress;
			video.ProcessingProgress = progress;
			video.EditDate = DateTime.Now;
			await Update(id, video);
			Console.WriteLine(
				DateTime.Now.ToLongTimeString()
					+ "   "
					+ id
					+ ": "
					+ pastProgress.ToString()
					+ " => "
					+ progress.ToString()
			);
		}

		public async Task ProccessVideoFile(string id, string path)
		{
			try
			{
				VideoMetadata video = await GetById(id);

				if (video == null)
					throw new Exception("Video does not exist!");

				if (video.ProcessingProgress != ProcessingProgress.Uploaded)
					throw new Exception("Video is in unacceptable state (" + video.ProcessingProgress.ToString() + ")");

				if (!System.IO.File.Exists(path))
					throw new Exception("Original video file does not exist!");

				string? newPath = videoManager.GetReadyFilePath(id);
				if (newPath == null)
					throw new Exception("Unable to create path for converted video file!");

				await ChangeVideoProcessingProgress(id, ProcessingProgress.Processing);

				await Cli.Wrap("ffmpeg")
					.WithArguments(
						new[]
						{
							"-i",
							path,
							"-map_metadata",
							"-1",
							"-c:v",
							"libx264",
							"-preset",
							"faster",
							"-crf",
							"30",
							"-c:a",
							"ac3",
							"-b:a",
							"128k",
							newPath
						}
					)
					.ExecuteBufferedAsync();

				if (!System.IO.File.Exists(newPath))
					throw new Exception("After successful conversion, output file does not exist!");

				System.IO.File.Delete(path);
				if (System.IO.File.Exists(path))
					throw new Exception("Unable to delete original video file!");

				await ChangeVideoProcessingProgress(id, ProcessingProgress.Ready);
			}
			catch (Exception e)
			{
				await ChangeVideoProcessingProgress(id, ProcessingProgress.FailedToProcess);

				String errorMessage = DateTime.Now.ToString() + "   " + id + "   " + e.Message;
				Console.WriteLine(errorMessage);

				string? location = videoManager.GetStorageDirectory();
				if (location == null)
					return;

				using (StreamWriter sw = File.AppendText(Path.Combine(location, id + "_error_log.txt")))
					sw.WriteLine(errorMessage);
			}
		}

		public async Task SetThumbnail(VideoMetadata video, VideoBaseDto videoDto)
		{
			if (videoDto.Thumbnail != null)
			{
				video.Thumbnail = "api/thumbnail/";
				video.Thumbnail += await UploadImage(videoDto.Title + " - thumbnail", videoDto.Thumbnail);
			}
		}

		public async Task<byte[]> GetThumbnailBytes(string id)
		{
			return await _bucket.DownloadAsBytesAsync(ObjectId.Parse(id));
		}

		public async Task<IEnumerable<VideoMetadata>> GetVideosByUserId(string id, bool isAuthor)
		{
			return await Collection
				.Find(x => x.AuthorId == id && (x.Visibility == VideoVisibility.Public || isAuthor))
				.ToListAsync();
		}

		public async Task<IEnumerable<VideoMetadata>> GetSubscribedVideos(IEnumerable<string> creatorsIds)
		{
			// zwracamy tylko publiczne filmy
			var videos = await Collection
				.Find(video => creatorsIds.Contains(video.AuthorId) && video.Visibility == VideoVisibility.Public)
				.ToListAsync();

			return videos.OrderByDescending(video => video.UploadDate);
		}

		public async Task<IEnumerable<VideoMetadata>> GetVideos(IEnumerable<string> videosIDs, string userID)
		{
			var toReturn = new List<VideoMetadata>();

			foreach (var v in videosIDs)
			{
				VideoMetadata video = await GetById(v);
				if (video == null)
				{
					video = new VideoMetadata();
					video.Title = "Wideo zostało usunięte.";
				}
				else if (video.Visibility == VideoVisibility.Private && video.AuthorId != userID)
				{
					video = new VideoMetadata();
					video.Title = "Wideo jest niedostępne.";
				}
				toReturn.Add(video);
			}

			return toReturn;
		}

		public async Task<IEnumerable<VideoMetadata>> GetAllVisibleVideos(string userId)
		{
			return await Collection
				.Find(video => video.Visibility == VideoVisibility.Public || video.AuthorId == userId)
				.ToListAsync();
		}

		public async Task<VideoMetadata> UpdateViewCount(string id, int value)
		{
			var update = Builders<VideoMetadata>.Update.Inc(u => u.ViewCount, value);
			await Collection.UpdateOneAsync(video => video.Id == id, update);
			return await GetById(id);
		}

		public async Task<string> GetDuration(string id)
		{
			string newPath = videoManager.GetReadyFilePath(id)!;
			BufferedCommandResult? result = await Cli.Wrap("ffmpeg")
				.WithArguments(new[] { "-i", newPath })
				.WithValidation(CommandResultValidation.None)
				.ExecuteBufferedAsync();

			List<string> splitted = result.StandardError.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
			splitted.RemoveAll(x => !Regex.IsMatch(x, @"\d*\d\d:\d\d:\d\d.\d\d"));

			if (splitted.Count != 1)
				return String.Empty;

			var checkHourArr = (splitted[0].Substring(0, splitted[0].Length - 4)).Split(':');
			if (checkHourArr.Length != 3)
				return String.Empty;

			if (Int32.Parse(checkHourArr[0]) != 0)
				return checkHourArr[0] + ":" + checkHourArr[1] + ":" + checkHourArr[2];
			else
				return checkHourArr[1] + ":" + checkHourArr[2];
		}

		public async Task UpdateVideoDuration(string id, string duration)
		{
			var update = Builders<VideoMetadata>.Update.Set(v => v.Duration, duration);
			await Collection.UpdateOneAsync(v => v.Id == id, update);
		}

		public async Task ProcessAndAddDuration(string id, string path)
		{
			await ProccessVideoFile(id, path);
			string duration = await GetDuration(id);
			await UpdateVideoDuration(id, duration);
		}
	}
}
