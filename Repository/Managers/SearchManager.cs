using Entities.Data.Interfaces;
using Entities.Data.Playlist;
using Entities.Data.User;
using Entities.Data.Video;
using Entities.Enums;
using FuzzyString;

namespace Repository.Managers
{
	public class SearchManager
	{
		private static readonly List<FuzzyStringComparisonOptions> _searchOptions =
			new()
			{
				FuzzyStringComparisonOptions.UseOverlapCoefficient,
				FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
				FuzzyStringComparisonOptions.UseLongestCommonSubstring,
			};

		private static readonly FuzzyStringComparisonTolerance _searchTolerance = FuzzyStringComparisonTolerance.Normal;

		private static Func<T, bool> CreatePredicate<T>(string query, Func<T, string[]> getValues)
		{
			return x =>
			{
				foreach (var value in getValues(x))
				{
					if (value.ApproximatelyEquals(query, _searchOptions, _searchTolerance))
					{
						return true;
					}
				}
				return false;
			};
		}

		private static List<T> FuzzySearch<T>(IEnumerable<T> collection, string query, Func<T, string[]> getValues)
			where T : ISearchable
		{
			var predicate = CreatePredicate(query, getValues);

			return collection.Where(predicate).ToList();
		}

		public List<UserDto> SearchUsers(IEnumerable<UserDto> users, string query)
		{
			return FuzzySearch(users, query, x => new string[] { x.Nickname });
		}

		public List<VideoMetadataDto> SearchVideos(IEnumerable<VideoMetadataDto> videos, string query)
		{
			return FuzzySearch(
				videos,
				query,
				x =>
				{
					var values = ((string[])x.Tags.ToArray().Clone()).ToList();
					values.Add(x.Title);
					values.Add(x.AuthorNickname);
					return values.ToArray();
				}
			);
		}

		public List<PlaylistBaseDto> SearchPlaylists(IEnumerable<PlaylistBaseDto> playlists, string query)
		{
			return FuzzySearch(playlists, query, x => new string[] { x.Name });
		}

		public List<UserDto> SortUsersBySubs(List<UserDto> userDtos)
		{
			return userDtos.OrderByDescending(x => x.SubscriptionsCount).ToList();
		}

		public List<VideoMetadataDto> SortAndFilterVideos(
			List<VideoMetadataDto> videoMetadataDtos,
			SortingCriterion sortingCriterion,
			SortingType sortingType,
			DateOnly? beginDate,
			DateOnly? endDate
		)
		{
			if (beginDate != null)
				videoMetadataDtos.RemoveAll(x => DateOnly.FromDateTime(x.UploadDate) < beginDate);
			if (endDate != null)
				videoMetadataDtos.RemoveAll(x => DateOnly.FromDateTime(x.UploadDate) > endDate);

			switch (sortingCriterion)
			{
				case SortingCriterion.Popularity:
					if (sortingType == SortingType.Ascending)
						return videoMetadataDtos.OrderBy(x => x.ViewCount).ToList();
					else
						return videoMetadataDtos.OrderByDescending(x => x.ViewCount).ToList();

				case SortingCriterion.Alphabetical:
					if (sortingType == SortingType.Ascending)
						return videoMetadataDtos.OrderBy(x => x.Title).ToList();
					else
						return videoMetadataDtos.OrderByDescending(x => x.Title).ToList();

				case SortingCriterion.PublishDate:
					if (sortingType == SortingType.Ascending)
						return videoMetadataDtos.OrderBy(x => x.UploadDate).ToList();
					else
						return videoMetadataDtos.OrderByDescending(x => x.UploadDate).ToList();

				default:
					throw new Exception("Unexpected value in sortingCriterion parameter");
			}
		}
	}
}
