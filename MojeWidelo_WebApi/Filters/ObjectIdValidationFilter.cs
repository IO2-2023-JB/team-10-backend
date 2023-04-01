using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;

namespace MojeWidelo_WebApi.Filters
{
	/// <summary>
	/// Filtr do walidacji id, sprawdza format stringa.
	/// Używać do endpointów, które mają {id} w route
	/// </summary>
	public class ObjectIdValidationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (
				!context.ActionArguments.ContainsKey("id")
				|| !ObjectId.TryParse(context.ActionArguments["id"]!.ToString(), out _)
			)
			{
				context.Result = new BadRequestResult();
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
