using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter
    {
        
        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        //injecting ILogger
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }
        //after execution of Action method
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(ResponseHeaderActionFilter),nameof(OnActionExecuted));
            throw new NotImplementedException();
        }

        //before execution of Action Method
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
            throw new NotImplementedException();
        }
    }
}
