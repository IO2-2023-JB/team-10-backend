﻿using Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data.Video
{
	public class ReactionDto
	{
		[EnumDataType(typeof(ReactionType))]
		[Required]
		public ReactionType ReactionType { get; set; }
	}
}
