using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logic.Helpers.ExceptionFolder
{
    public class CustomExceptionFilterAttribute : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new RedirectToActionResult("Index", "Error", null);
            context.ExceptionHandled = true;
        }
    }
}
