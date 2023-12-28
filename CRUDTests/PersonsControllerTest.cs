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
        private readonly ICountriesService _countriesService;
        private readonly IPersonsService _personsService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ILogger<PersonsController>> _loggerMock;
        private readonly ILogger<PersonsController> _logger;
        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;

            _loggerMock = new Mock<ILogger<PersonsController>>();
            _logger = _loggerMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> personsResponseList = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService,_logger);

            //mocking GetFilteredPersons() Service method, as it is invoked in the Controller Index Action-Method
            _personsServiceMock.Setup(
                temp => temp.GetFilteredPersons(
                    It.IsAny<string>(),It.IsAny<string>()))
                .ReturnsAsync(personsResponseList);

            //mocking GetSortedPersons()
            _personsServiceMock.Setup(
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
            _countriesServiceMock.Setup(
                temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _personsServiceMock.Setup(
                temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService, _logger);

            //Act
            IActionResult result = await personsController.Create(personAddRequest);

            //Assert
            RedirectToActionResult redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            redirectToActionResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
