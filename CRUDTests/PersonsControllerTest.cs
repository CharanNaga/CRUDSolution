﻿using AutoFixture;
using CRUDExample.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IFixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;
        }
        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> personsResponseList = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService);

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
    }
}
