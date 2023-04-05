using CliWrap;
using Contracts;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;
using System.Runtime.InteropServices;
using CliWrap.Buffered;

namespace Repository
{
	public class VideoRepository : RepositoryBase<VideoMetadata>, IVideoRepository
	{
		public VideoRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.VideoCollectionName) { }

		public string? CreateNewPath(string id, string fileName)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			string extension = Path.GetExtension(fileName);
			return Path.Combine(location, id + "_original" + extension);
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

		public string? GetStorageDirectory()
		{
			string location;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				location = Environment.GetEnvironmentVariable("MojeWideloStorage", EnvironmentVariableTarget.Machine)!;

				if (string.IsNullOrEmpty(location))
					return null;
			}
			else
			{
				//WILL BE IMPLEMENTED PROPERLY IN SPRINT 3
				location = "/home/ubuntu/video-storage";
			}

			return location;
		}

		public string? GetReadyFilePath(string id)
		{
			string? location = GetStorageDirectory();
			if (location == null)
				return null;

			return Path.Combine(location, id + ".mp4");
		}

		public async void ProccessVideoFile(string id, string path)
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

				string? newPath = GetReadyFilePath(id);
				if (newPath == null)
					throw new Exception("Unable to create path for converted video file!");

				await ChangeVideoProcessingProgress(id, ProcessingProgress.Processing);

				await Cli.Wrap("ffmpeg").WithArguments(new[] { "-i", path, newPath }).ExecuteBufferedAsync();

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

				string? location = GetStorageDirectory();
				if (location == null)
					return;

				using (StreamWriter sw = File.AppendText(Path.Combine(location, id + "_error_log.txt")))
					sw.WriteLine(errorMessage);
			}
		}
	}
}
