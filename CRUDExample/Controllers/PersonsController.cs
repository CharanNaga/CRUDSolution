using CRUDExample.Filters.ActionFilters;
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
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = new object[] { "CustomKey-FromController", "CustomValue-FromController",3 },Order = 3)] //passing arguments to filter constructor helpful in response headers.
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
        [TypeFilter(typeof(PersonsListActionFilter),Order =4)] //creates an obj of PersonsListActionFilter & attaches to the Index Action Method
        [TypeFilter(typeof(ResponseHeaderActionFilter),Arguments = new object[] {"CustomKey-FromAction","CustomValue-FromAction",1},Order = 1)] //passing arguments to filter constructor helpful in response headers.
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            //writing log message to indicate that ww are in Index action method
            _logger.LogInformation("Index action method of PersonsController");

            //writing Index() method parameters using Debug
            _logger.LogDebug(
                $"searchBy:{searchBy}, searchString:{searchString}, sortBy:{sortBy}, sortOrder:{sortOrder}");


            //Searching

            //Written this functionality in PersonsListActionFilter
            //ViewBag.SearchFields = new Dictionary<string, string>()
            //{//property name & display name
            //    {nameof(PersonResponse.PersonName), "Person Name" },
            //    {nameof(PersonResponse.Email), "Email" },
            //    {nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            //    {nameof(PersonResponse.Gender), "Gender" },
            //    {nameof(PersonResponse.CountryID), "Country" },
            //    {nameof(PersonResponse.Address), "Address" },
            //};

            List<PersonResponse> persons = await _personsService.GetFilteredPersons(searchBy, searchString);

            //Written these ViewBag fields functionality in PersonsListActionFilter
            //ViewBag.CurrentSearchBy = searchBy;
            //ViewBag.CurrentSearchString = searchString;

            //Sorting
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons, sortBy, sortOrder);

            //Written these ViewBag fields functionality in PersonsListActionFilter
            //ViewBag.CurrentSortBy = sortBy;
            //ViewBag.CurrentSortOrder = sortOrder.ToString();

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
        public async Task<IActionResult> Create(PersonAddRequest request)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp =>
                    new SelectListItem()
                    {
                        Text = temp.CountryName,
                        Value = temp.CountryID.ToString()
                    });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(request);
            }
            //calling service
            PersonResponse personResponse = await _personsService.AddPerson(request);
            //navigate to Index(), after adding new person and it makes another get request to persons/index.
            return RedirectToAction("Index", "Persons");
        }
        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(personID);
            if(response == null)
            {
                return RedirectToAction("Index");
            }
            PersonUpdateRequest updateRequest =  response.ToPersonUpdateRequest();
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
        public async Task<IActionResult> Edit(PersonUpdateRequest updateRequest)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(updateRequest.PersonID);
            if(response == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                PersonResponse updatePerson = await _personsService.UpdatePerson(updateRequest);
                return RedirectToAction("Index","Persons");
            }
           else
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp =>
                    new SelectListItem()
                    {
                        Text = temp.CountryName,
                        Value = temp.CountryID.ToString()
                    });
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(response.ToPersonUpdateRequest());
            }  
        }

        [Route("[action]/{personID}")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? response = await _personsService.GetPersonByPersonID(personID);
            if(response == null)
                return RedirectToAction("Index");
            return View(response);
        }

        [Route("[action]/{personID}")]
        [HttpPost]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse= await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);
            if(personResponse == null)
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
                PageMargins = new Margins() { Top=20, Bottom=20,Left = 20,Right = 20 },
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