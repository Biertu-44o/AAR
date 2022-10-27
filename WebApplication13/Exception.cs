using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

    public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine(context.Result);
            context.Result = new RedirectResult("~/ErrorRegistrationPage.html");
            context.ExceptionHandled = true;
        
        }
    }
