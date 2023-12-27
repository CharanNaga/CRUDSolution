using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    //Parameterized Action Filter as it receives args key & value
    public class ResponseHeaderActionFilter : IActionFilter,IOrderedFilter
    {
        
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;

        public int Order { get; set; }

        //injecting ILogger
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            Key = key;
            Value = value;
            Order = order;
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
