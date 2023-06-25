using FuzzyString;

namespace FuzzySearch
{
	public static class StringComparisonExtension
	{
		public static double CompareStrings(
			this string source,
			string target,
			List<FuzzyStringComparisonOptions> options
		)
		{
			List<double> list = new();
			if (!options.Contains(FuzzyStringComparisonOptions.CaseSensitive))
			{
				source = source.Capitalize();
				target = target.Capitalize();
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseHammingDistance) && source.Length == target.Length)
			{
				list.Add(source.HammingDistance(target) / target.Length);
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseJaccardDistance))
			{
				list.Add(source.JaccardDistance(target));
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseJaroDistance))
			{
				list.Add(source.JaroDistance(target));
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseJaroWinklerDistance))
			{
				list.Add(source.JaroWinklerDistance(target));
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseNormalizedLevenshteinDistance))
			{
				list.Add(
					Convert.ToDouble(source.NormalizedLevenshteinDistance(target))
						/ Convert.ToDouble(
							Math.Max(source.Length, target.Length) - source.LevenshteinDistanceLowerBounds(target)
						)
				);
			}
			else if (options.Contains(FuzzyStringComparisonOptions.UseLevenshteinDistance))
			{
				list.Add(
					Convert.ToDouble(source.LevenshteinDistance(target))
						/ Convert.ToDouble(source.LevenshteinDistanceUpperBounds(target))
				);
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubsequence))
			{
				list.Add(
					1.0
						- Convert.ToDouble(
							(double)source.LongestCommonSubsequence(target).Length
								/ Convert.ToDouble(Math.Min(source.Length, target.Length))
						)
				);
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseLongestCommonSubstring))
			{
				list.Add(
					1.0
						- Convert.ToDouble(
							(double)source.LongestCommonSubstring(target).Length
								/ Convert.ToDouble(Math.Min(source.Length, target.Length))
						)
				);
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseSorensenDiceDistance))
			{
				list.Add(source.SorensenDiceDistance(target));
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseOverlapCoefficient))
			{
				list.Add(1.0 - source.OverlapCoefficient(target));
			}

			if (options.Contains(FuzzyStringComparisonOptions.UseRatcliffObershelpSimilarity))
			{
				list.Add(1.0 - source.RatcliffObershelpSimilarity(target));
			}

			var score = list.Average();
			return score;
		}
	}
}
