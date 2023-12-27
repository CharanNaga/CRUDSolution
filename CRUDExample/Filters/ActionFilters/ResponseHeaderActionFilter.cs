using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    //Parameterized Action Filter as it receives args key & value
    public class ResponseHeaderActionFilter : IActionFilter
    {
        
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;

        //injecting ILogger
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value)
        {
            _logger = logger;
            Key = key;
            Value = value;
        }
        //after execution of Action method
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(ResponseHeaderActionFilter),nameof(OnActionExecuted));
            context.HttpContext.Response.Headers[Key] = Value;

        }

        //before execution of Action Method
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));

        }
    }
}
