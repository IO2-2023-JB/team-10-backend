﻿using Microsoft.AspNetCore.Mvc;
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
		private const string idKey = "id";

		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (
				!context.ActionArguments.ContainsKey(idKey)
				|| !ObjectId.TryParse(context.ActionArguments[idKey]!.ToString(), out _)
			)
			{
				context.Result = new BadRequestResult();
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
