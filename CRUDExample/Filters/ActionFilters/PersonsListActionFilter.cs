using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

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

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                //validate searchBy parameter value
                if(!string.IsNullOrEmpty(searchBy) )
                {
                    List<string> searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Address)
                    };
                    //resetting searchBy value
                    if(searchByOptions.Any(temp=>temp == searchBy) == false)
                    {
                        _logger.LogInformation("searchBy actual value {searchBy}", searchBy);
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        _logger.LogInformation("searchBy updated value {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }
        }
    }
}
