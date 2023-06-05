using Microsoft.AspNetCore.Mvc.Filters;

namespace MojeWidelo_WebApi.Filters
{
	public class RenameParameterFilter : IActionFilter
	{
		private const string idKey = "id";
		private readonly string[] keysToReplace = new string[] { "subId", "creatorId" };

		public void OnActionExecuting(ActionExecutingContext context)
		{
			foreach (var key in keysToReplace)
			{
				if (context.ActionArguments.TryGetValue(key, out var value))
				{
					context.ActionArguments.Add(idKey, value);
					break;
				}
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
