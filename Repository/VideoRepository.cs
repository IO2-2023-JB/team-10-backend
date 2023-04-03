﻿using Contracts;
using Entities.Data.Video;
using Entities.DatabaseUtils;
using Entities.Enums;
using Entities.Models;
using System.Runtime.InteropServices;

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

		public async void ChangeVideoProcessingProgress(
			IRepositoryWrapper _repository,
			string id,
			ProcessingProgress progress
		)
		{
			VideoMetadata video = await _repository.VideoRepository.GetById(id);
			video.ProcessingProgress = progress;
			video.EditDate = DateTime.Now;
			await _repository.VideoRepository.Update(id, video);
		}
	}
}
