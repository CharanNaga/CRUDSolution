using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    //Parameterized Action Filter as it receives args key & value
    public class ResponseHeaderActionFilter : IAsyncActionFilter,IOrderedFilter
    {
        
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;

        public int Order { get; set; }

        //injecting ILogger
        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            _key = key;
            _value = value;
            Order = order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //before execution
            _logger.LogInformation("{FilterName}.{MethodName} method - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            await next(); //calls subsequent filter or action method

            //after execution
            _logger.LogInformation("{FilterName}.{MethodName} method - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[_key] = _value;
        }
    }
}
