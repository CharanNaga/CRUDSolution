using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ResultFilters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<PersonsListResultFilter> _logger;
        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            //before execution
            _logger.LogInformation("{FilterName}.{MethodName} - before execution",nameof(PersonsListResultFilter),nameof(OnResultExecutionAsync));
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm"); //placing in before logic from after logic, as when the response is started to communicate with client, we can't add new headers
            
            await next(); //call the subsequent filter or IActionResult

            //after execution
            _logger.LogInformation("{FilterName}.{MethodName} - after execution", nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
           
        }
    }
}
