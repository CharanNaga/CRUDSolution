using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter
    {
        //after execution of Action method
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        //before execution of Action Method
        public void OnActionExecuting(ActionExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
