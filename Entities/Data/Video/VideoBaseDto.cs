﻿using Entities.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
	public abstract class VideoBaseDto
	{
		public string Title { get; set; }

		public string? Description { get; set; }

		public IFormFile? Thumbnail { get; set; }

		public IEnumerable<string>? Tags { get; set; }

		[EnumDataType(typeof(VideoVisibility))]
		public VideoVisibility Visibility { get; set; }
	}
}