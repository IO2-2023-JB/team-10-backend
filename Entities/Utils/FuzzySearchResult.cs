using Entities.Data.Interfaces;

namespace Entities.Utils
{
	public class FuzzySearchResult<T>
		where T : ISearchable
	{
		public T Data { get; set; }
		public double? Score { get; set; }

		public FuzzySearchResult(T data, double? score)
		{
			Data = data;
			Score = score;
		}
	}
}
