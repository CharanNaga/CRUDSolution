using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
            _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonsListActionFilter),nameof(OnActionExecuted));

            //Logic for Setting ActionMethod parameters into ViewBag for minimizing code in Controller
            PersonsController personsController = (PersonsController) context.Controller;
            var parameters =(IDictionary<string,object?>?) context.HttpContext.Items["Arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy")) //Equivalent to assigning searchBy value to viewdata obj in controller
                {
                    personsController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString")) //Equivalent to assigning searchString value to viewdata obj in controller
                {
                    personsController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy")) //Equivalent to assigning sortBy value to viewdata obj in controller
                {
                    personsController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortBy"] = nameof(PersonResponse.PersonName);
                }

                if (parameters.ContainsKey("sortOrder")) //Equivalent to assigning sortOrder value to viewdata obj in controller
                {
                    personsController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
                else
                {
                    personsController.ViewData["CurrentSortOrder"] = nameof(SortOrderOptions.ASC);
                }
            }
            //Searching
            personsController.ViewBag.SearchFields = new Dictionary<string, string>()
            {//property name & display name
                {nameof(PersonResponse.PersonName), "Name" },
                {nameof(PersonResponse.Email), "Email" },
                {nameof(PersonResponse.DateOfBirth), "DOB" },
                {nameof(PersonResponse.Gender), "Gender" },
                {nameof(PersonResponse.CountryID), "Country" },
                {nameof(PersonResponse.Address), "Address" },
            };
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Items["Arguments"] = context.ActionArguments; //storing ActionArguments into a dictionary so that can be accessible in OnExecuted also, as that context doesnt support ActionArguments property

            //Add "before execution" logic here
            _logger.LogInformation("{FilterName}.{MethodName} Method", nameof(PersonsListActionFilter), nameof(OnActionExecuting));

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
