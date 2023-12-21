using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDExample.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Add "after execution" logic here
            _logger.LogInformation($"{nameof(PersonsListActionFilter)}.{nameof(OnActionExecuted)} Filter method");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Add "before execution" logic here
            _logger.LogInformation($"{nameof(PersonsListActionFilter)}.{nameof(OnActionExecuting)} Filter method");
        }
    }
}
