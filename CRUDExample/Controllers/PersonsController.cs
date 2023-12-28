using CRUDExample.Filters;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.AuthorizationFilter;
using CRUDExample.Filters.ExceptionFilters;
using CRUDExample.Filters.ResourceFilters;
using CRUDExample.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    //[Route("persons")] //applied for all action methods in this controller, so we can remove persons/index as just index
    [Route("[controller]")] //Same as above but implementing using Route Token. In future, if controller name changes then this route token is helpful.
    //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "CustomKey-FromController", "CustomValue-FromController", 3 }, Order = 3)] //passing arguments to filter constructor helpful in response headers.
    //[ResponseHeaderActionFilter("CustomKey-FromController", "CustomValue-FromController", 3)]

    [ResponseHeaderFilterFactory("CustomKey-FromController", "CustomValue-FromController", 3)]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        //constructor
        public PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;
            _logger = logger;
        }

        //[Route("index")] //read as "persons/index"
        [Route("[action]")] //Same as above but implemented using Route Token. Holds good, when Action Method Name & Url Name are same otherwise explicitly mention the url string.
        [Route("/")] // overriden as just "/", / indicates overriding default url
        //[TypeFilter(typeof(PersonsListActionFilter), Order = 4)] //creates an obj of PersonsListActionFilter & attaches to the Index Action Method
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)] //creates an obj of PersonsListActionFilter & attaches to the Index Action Method but need to add this filter as a service in Ioc container
        //[TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "CustomKey-FromAction", "CustomValue-FromAction", 1 }, Order = 1)] //passing arguments to filter constructor helpful in response headers.
        //[ResponseHeaderActionFilter("CustomKey-FromAction", "CustomValue-FromAction", 1)]

        [ResponseHeaderFilterFactory("CustomKey-FromAction", "CustomValue-FromAction", 1)]

        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter] //skipping the functionality of filter for Index Action Method only
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //writing log message to indicate that ww are in Index action method
            _logger.LogInformation("Index action method of PersonsController");

            //writing Index() method parameters using Debug
            _logger.LogDebug(
                $"searchBy:{searchBy}, searchString:{searchString}, sortBy:{sortBy}, sortOrder:{sortOrder}");

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //Sorting
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);
            return View(sortedPersons);
        }

        //Executes when user clicks on hyperlink "Create Person", while opening the create view.
        //[Route("create")]
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });
            return View();
        }

        //Executes when user click on submit button in create view.
        //[Route("create")]
        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            //calling service
            PersonResponse personResponse = await _personsService.AddPerson(personRequest);
            //navigate to Index(), after adding new person and it makes another get request to persons/index.
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        //[TypeFilter(typeof(TokenResultFilter))] //commenting this for demonstration of AlwaysRunResultFilter
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(personID);
            if (response == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest updateRequest = response.ToPersonUpdateRequest();
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
                new SelectListItem()
                {
                    Text = temp.CountryName,
                    Value = temp.CountryID.ToString()
                });
            return View(updateRequest);
        }
        [Route("[action]/{personID}")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        //[TypeFilter(typeof(TokenAuthorizationFilter))] //comment this if TokenResultFilter is commented. Otherwise, it shows 401 error
        //[TypeFilter(typeof(PersonAlwaysRunResultFilter))] //Commented and placed in controller level for demonstrating SkipFilter
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(personRequest.PersonID);
            if (response == null)
            {
                return RedirectToAction("Index");
            }
            PersonResponse updatePerson = await _personsService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(personID);
            if (response == null)
                return RedirectToAction("Index");
            return View(response);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");
            await _personsService.DeletePerson(personResponse.PersonID);
            return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //get persons list
            List<PersonResponse> persons = await _personsService.GetAllPersons();

            //return view as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Margins() { Top = 20, Bottom = 20, Left = 20, Right = 20 },
                PageOrientation = Orientation.Landscape,
            };
        }
        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream stream = await _personsService.GetPersonsCSV();
            return File(stream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream stream = await _personsService.GetPersonsExcel();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}