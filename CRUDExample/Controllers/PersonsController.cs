using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personsService = personsService;
            _countriesService = countriesService;
        }

        [Route("persons/index")]
        [Route("/")]
        public IActionResult Index(string searchBy,string? searchString,string sortBy=nameof(PersonResponse.PersonName),SortOrderOptions sortOrder=SortOrderOptions.ASC)
        {
            //Searching
            ViewBag.SearchFields = new Dictionary<string, string>()
            {//property name & display name
                {nameof(PersonResponse.PersonName), "Person Name" }, 
                {nameof(PersonResponse.Email), "Email" },
                {nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                {nameof(PersonResponse.Gender), "Gender" },
                {nameof(PersonResponse.CountryID), "Country" },
                {nameof(PersonResponse.Address), "Address" },
            };
            List<PersonResponse> persons = _personsService.GetFilteredPersons(searchBy,searchString);
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            //Sorting
            List<PersonResponse> sortedPersons = _personsService.GetSortedPersons(persons,sortBy,sortOrder);
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrder.ToString();

            return View(sortedPersons);
        }

        //Executes when user clicks on hyperlink "Create Person", while opening the create view.
        [Route("persons/create")]
        [HttpGet]
        public IActionResult Create()
        {
            List<CountryResponse> countries = _countriesService.GetAllCountries();
            ViewBag.Countries = countries;
            return View();
        }

        //Executes when user click on submit button in create view.
        [Route("persons/create")]
        [HttpPost]
        public IActionResult Create(PersonAddRequest request)
        {
            if(!ModelState.IsValid)
            {
                List<CountryResponse> countries = _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }
            //calling service
            PersonResponse personResponse = _personsService.AddPerson(request);
            //navigate to Index(), after adding new person and it makes another get request to persons/index.
            return RedirectToAction("Index","Persons");
        }
    }
}
