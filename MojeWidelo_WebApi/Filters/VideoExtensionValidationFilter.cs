using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MojeWidelo_WebApi.Filters
{
	/// <summary>
	///     Filtr do HttpPost dla uploadu video
	///     Filtr sprawdza czy rozszerzenie pliku video jest akceptowalne
	///     Dodać nagłówek ServiceFilter do metody w kontrolerze:
	///     [ServiceFilter(typeof(ModelValidationFilter))]
	/// </summary>
	public class VideoExtensionValidationFilter : IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ActionArguments.ContainsKey("videoFile"))
				context.Result = new BadRequestResult();

			string extension = Path.GetExtension(((IFormFile)context.ActionArguments["videoFile"]!).FileName);
			if (extension == null)
				context.Result = new BadRequestResult();

			string[] acceptedExtensions = { ".mkv", ".mp4", ".avi", ".webm" };

			if (!acceptedExtensions.Contains(extension!.ToLower()))
				context.Result = new BadRequestResult();
		}

		public void OnActionExecuted(ActionExecutedContext context) { }
	}
}
