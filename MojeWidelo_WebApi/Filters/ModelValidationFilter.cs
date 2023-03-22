using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Bson;
using System.Net.Mime;

namespace MojeWidelo_WebApi.Filters
{
    /// <summary>
    ///     Filtr do HttpPut i HttpPost walidujący przesyłanego obiektu
    /// </summary>
    /// <example>
    ///     TYLKO dla HttpPOST i HttpPUT!!!
    ///     Dodać nagłówek ServiceFilter do metody w kontrolerze:
    ///     [HttpPost]
    ///     [ServiceFilter(typeof(ModelValidationFilter))]
    ///     public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
    /// </example>
    public class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestResult();
            }
        }
        
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
