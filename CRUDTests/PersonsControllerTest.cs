using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly Mock<ICountriesGetterService> _countriesGetterServiceMock;

        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsSorterService _personsSorterService;

        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly ILogger<PersonsController> _logger;
        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesGetterServiceMock = new Mock<ICountriesGetterService>();
            _countriesGetterService = _countriesGetterServiceMock.Object;

            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();

            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> personsResponseList = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(
                _personsGetterService,_personsAdderService,_personsUpdaterService, 
                _personsDeleterService,_personsSorterService,_countriesGetterService,_logger);

            //mocking GetFilteredPersons() Service method, as it is invoked in the Controller Index Action-Method
            _personsGetterServiceMock.Setup(
                temp => temp.GetFilteredPersons(
                    It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(personsResponseList);

            //mocking GetSortedPersons()
            _personsSorterServiceMock.Setup(
                temp=>temp.GetSortedPersons(
                    It.IsAny<List<PersonResponse>>(),It.IsAny<string>(),It.IsAny<SortOrderOptions>()
                    )).ReturnsAsync(personsResponseList);

            //Act
            //through autofixture we are invoking controller's Index action method with some data
            IActionResult result = await personsController.Index(
                _fixture.Create<string>(), _fixture.Create<string>(),
                _fixture.Create<string>(), _fixture.Create<SortOrderOptions>()
                );

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(personsResponseList);
        }
        #endregion

        #region Create
        [Fact]
        //1.HttpPost Action method for Create with no model state errors
        public async Task Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();
            PersonResponse personResponse = _fixture.Create<PersonResponse>();
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            //mocking GetAllCountries() & AddPerson()
            _countriesGetterServiceMock.Setup(
                temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsAdderServiceMock.Setup(
                temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService, _personsAdderService, _personsUpdaterService,
                _personsDeleterService, _personsSorterService, _countriesGetterService, _logger);

            //Act
            IActionResult result = await personsController.Create(personAddRequest);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
