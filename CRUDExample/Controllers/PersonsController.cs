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
    //[TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonAlwaysRunResultFilter))]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ILogger<PersonsController> _logger;

        //constructor
        public PersonsController(IPersonsGetterService personsGetterService,IPersonsAdderService personsAdderService,IPersonsUpdaterService personsUpdaterService,IPersonsDeleterService personsDeleterService,IPersonsSorterService personsSorterService, ICountriesGetterService countriesGetterService, ILogger<PersonsController> logger)
        {
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderService;
            _personsUpdaterService = personsUpdaterService;
            _personsDeleterService = personsDeleterService;
            _personsSorterService = personsSorterService;
            _countriesGetterService = countriesGetterService;
            _logger = logger;
        }

        [Route("[action]")] 
        [Route("/")] 
        //[TypeFilter(typeof(PersonsListActionFilter), Order = 4)] 
        [ServiceFilter(typeof(PersonsListActionFilter), Order = 4)]

        [ResponseHeaderFilterFactory("CustomKey-FromAction", "CustomValue-FromAction", 1)]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
           
            _logger.LogInformation("Index action method of PersonsController");

            _logger.LogDebug(
                $"searchBy:{searchBy}, searchString:{searchString}, sortBy:{sortBy}, sortOrder:{sortOrder}");

            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);

            //Sorting
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            return View(sortedPersons);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem()
            {
                Text = temp.CountryName,
                Value = temp.CountryID.ToString()
            });
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(PersonCreateAndEditPostActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments = new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? response = await _personsGetterService.GetPersonByPersonID(personID);
            if (response == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest updateRequest = response.ToPersonUpdateRequest();
            List<CountryResponse> countries = await _countriesGetterService.GetAllCountries();
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
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? response = await _personsGetterService.GetPersonByPersonID(personRequest.PersonID);
            if (response == null)
            {
                return RedirectToAction("Index");
            }
            PersonResponse updatePerson = await _personsUpdaterService.UpdatePerson(personRequest);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? response = await _personsGetterService.GetPersonByPersonID(personID);
            if (response == null)
                return RedirectToAction("Index");
            return View(response);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");
            await _personsDeleterService.DeletePerson(personResponse.PersonID);
            return RedirectToAction("Index");
        }
        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            //get persons list
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

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
            MemoryStream stream = await _personsGetterService.GetPersonsCSV();
            return File(stream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream stream = await _personsGetterService.GetPersonsExcel();
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}