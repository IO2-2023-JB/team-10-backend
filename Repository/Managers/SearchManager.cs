using Entities.Data.Interfaces;
using Entities.Data.Playlist;
using Entities.Data.User;
using Entities.Data.Video;
using Entities.Enums;
using Entities.Utils;
using FuzzySearch;
using FuzzyString;

namespace Repository.Managers
{
	public class SearchManager
	{
		private static readonly List<FuzzyStringComparisonOptions> _searchOptions =
			new()
			{
				FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
				FuzzyStringComparisonOptions.UseLongestCommonSubstring,
				FuzzyStringComparisonOptions.UseSorensenDiceDistance,
				FuzzyStringComparisonOptions.UseRatcliffObershelpSimilarity,
			};

		private static readonly FuzzyStringComparisonTolerance _searchTolerance = FuzzyStringComparisonTolerance.Normal;

		private readonly Dictionary<string, double> cachedSearchScores;
		private readonly double tolerance;

		public SearchManager()
		{
			cachedSearchScores = new();
			tolerance = _searchTolerance switch
			{
				FuzzyStringComparisonTolerance.Strong => 0.25,
				FuzzyStringComparisonTolerance.Normal => 0.5,
				FuzzyStringComparisonTolerance.Weak => 0.75,
				FuzzyStringComparisonTolerance.Manual => 0.6,
				_ => default,
			};
		}

		private List<T> FuzzySearch<T>(IEnumerable<T> collection, string query, Func<T, string[]> getValues)
			where T : ISearchable
		{
			List<FuzzySearchResult<T>> searchResults = new();
			foreach (var item in collection)
			{
				double currentScore = tolerance;
				var values = getValues(item);
				foreach (var value in values)
				{
					if (!cachedSearchScores.TryGetValue(value, out double score))
					{
						score = value.CompareStrings(query, _searchOptions);
						cachedSearchScores.Add(value, score);
					}
					currentScore = Math.Min(currentScore, score);
				}

				if (currentScore < tolerance)
				{
					searchResults.Add(new FuzzySearchResult<T>(item, currentScore));
				}
			}

			var result = searchResults.OrderBy(x => x.Score).Select(x => x.Data).ToList();
			return result;
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

				case SortingCriterion.BestMatch:
					return videoMetadataDtos;

				default:
					throw new Exception("Unexpected value in sortingCriterion parameter");
			}
		}
	}
}
