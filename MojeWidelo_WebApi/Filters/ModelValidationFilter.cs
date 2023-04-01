using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MojeWidelo_WebApi.Filters
{
	/// <summary>
	///     Filtr do HttpPut i HttpPost walidujący przesyłanego obiektu
	///     Dodać nagłówek ServiceFilter do metody w kontrolerze:
	///     [ServiceFilter(typeof(ModelValidationFilter))]
	/// </summary>
	public class ModelValidationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				context.Result = new BadRequestResult();
			}
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
