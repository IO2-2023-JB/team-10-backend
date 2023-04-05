using CliWrap;
using Contracts;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using System.Text;
using CliWrap.Buffered;

namespace Repository
{
	public class VideoRepository : RepositoryBase<VideoMetadata>, IVideoRepository
	{
		public VideoRepository(IDatabaseSettings databaseSettings)
			: base(databaseSettings, databaseSettings.VideoCollectionName) { }

		public string? CreateNewPath(string id, string fileName)
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

			string extension = Path.GetExtension(fileName);
			return Path.Combine(location, id + "_original" + extension);
		}

		public async Task<bool> ChangeVideoProcessingProgress(string id, ProcessingProgress progress)
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
			return true;
		}

		public string? GetReadyFilePath(string id)
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

			return Path.Combine(location, id + ".mp4");
		}

		public async void ProccessVideoFile(string id, string path)
		{
			Exception e = new Exception(message: StatusCodes.Status500InternalServerError.ToString());
			VideoMetadata video = await GetById(id);

			if (video == null || video.ProcessingProgress != ProcessingProgress.Uploaded)
				throw e;

			if (!System.IO.File.Exists(path))
				throw e;

			string? newPath = GetReadyFilePath(id);
			if (newPath == null)
				throw e;

			BufferedCommandResult result;

			try
			{
				await ChangeVideoProcessingProgress(id, ProcessingProgress.Processing);
				result = await Cli.Wrap("ffmpeg").WithArguments(new[] { "-i", path, newPath }).ExecuteBufferedAsync();
			}
			catch (Exception)
			{
				await ChangeVideoProcessingProgress(id, ProcessingProgress.FailedToProcess);
				throw e;
			}

			if (!System.IO.File.Exists(newPath))
				throw e;

			System.IO.File.Delete(path);
			if (System.IO.File.Exists(path))
				throw e;

			await ChangeVideoProcessingProgress(id, ProcessingProgress.Ready);
			return;
		}
	}
}
