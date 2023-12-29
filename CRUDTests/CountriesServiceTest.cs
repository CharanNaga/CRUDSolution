using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        //private fields
        #region Private readonly fields
        private readonly ICountriesGetterService _countriesGetterService;
        private readonly ICountriesAdderService _countriesAdderService;

        private readonly ICountriesRepository _countriesRepository;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly IFixture _fixture;
        #endregion

        //constructor
        #region Constructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesGetterService = new CountriesGetterService(_countriesRepository);
            _countriesAdderService = new CountriesAdderService(_countriesRepository);
        }
        #endregion

        #region AddCountry
        //Four Requirements for Test..
        //1. When CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons,null as List<Person>)
                .Create();

            //mocking AddCountry()
            _countriesRepositoryMock.Setup(
                temp=>temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            //Assert
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };
            //Act
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //2. When CountryName is null, throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            Country country = _fixture.Build<Country>()
                .With(c => c.Persons, null as List<Person>)
                .Create();

            //mocking AddCountry()
            _countriesRepositoryMock.Setup(
                temp=>temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            //Act
            Func<Task> action = async () =>
            {
                await _countriesAdderService.AddCountry(request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //3. When CountryName is Duplicate, throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();

            Country country1 = request1.ToCountry();
            Country country2 = request2.ToCountry();

            //mocking AddCountry()
            _countriesRepositoryMock.Setup(
                temp=> temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

            //mocking GetCountryByCountryName() & Return null when GetCountryByCountryName is called
            _countriesRepositoryMock.Setup(
                temp=>temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse countryResponse = await _countriesAdderService.AddCountry(request1);

            //Act
            Func<Task> action = async () =>
            {
                //Return first country when GetCountryByCountryName is called
                _countriesRepositoryMock.Setup(
                    temp => temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country1);

                _countriesRepositoryMock.Setup(
                    temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(country1);

                await _countriesAdderService.AddCountry(request2);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //4. When proper CountryName is supplied, insert the country to the existing list of countries
        [Fact]
        public async Task AddCountry_FullCountryDetails_ToBeSuccessful()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Create<CountryAddRequest>();
            Country country = request.ToCountry();
            CountryResponse countryResponse = country.ToCountryResponse();

            //mocking AddCountry()
            _countriesRepositoryMock.Setup(
                temp=>temp.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(country);

            //mocking GetCountryByCountryName()
            _countriesRepositoryMock.Setup(
                temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            //Act
            CountryResponse responseFromAddCountry = await _countriesAdderService.AddCountry(request);
            country.CountryID = responseFromAddCountry.CountryID;
            countryResponse.CountryID = responseFromAddCountry.CountryID;

            //Assert
            responseFromAddCountry.CountryID.Should().NotBe(Guid.Empty);
            responseFromAddCountry.Should().BeEquivalentTo(countryResponse);
        }
        #endregion

        #region GetAllCountries
        //1. Without adding any country, list should be empty. List of countries should be empty before adding any countries.
        [Fact]
        public async Task GetAllCountries_EmptyList_ToBeEmptyList()
        {
            //Arrange
            List<Country> countries = new List<Country>();

            //mocking GetAllCountries()
            _countriesRepositoryMock.Setup(temp=>temp.GetAllCountries()).ReturnsAsync(countries);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesGetterService.GetAllCountries();

            //Assert
            //Assert.Empty(actualCountryResponseList); //if it is empty, test case pass.
            actualCountryResponseList.Should().BeEmpty();
        }

        //2. If we add few countries, then same countries should be returned.
        [Fact]
        public async Task GetAllCountries_ShouldHaveFewCountries()
        {
            //Arrange
            List<Country> countries = new List<Country>()
            {
               _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create(),
              _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create(),
            };

            //Act
            List<CountryResponse> countriesListFromAddCountry = countries.Select(temp=>temp.ToCountryResponse()).ToList();

            //mocking GetAllCountries()
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(countries);

            List<CountryResponse> actualContriesListFromGetCountry = await _countriesGetterService.GetAllCountries();

            //Assert
            actualContriesListFromGetCountry.Should().BeEquivalentTo(countriesListFromAddCountry);
        }
        #endregion

        #region GetCountryByCountryID
        //1. if CountryID supplied is null, it should return null as CountryResponse.
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Arrange
            Guid? countryId = null;

            //mocking GetCountryByCountryID()
            _countriesRepositoryMock.Setup(
                temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(null as Country);

            //Act 
            CountryResponse? countryResponseFromGetCountryById = await _countriesGetterService.GetCountryByCountryID(countryId);

            //Assert
            countryResponseFromGetCountryById.Should().BeNull();
        }

        //2. if valid CountryID is supplied, return matching CountryDetails as CountryResponse Object.
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = _fixture.Build<Country>().With(temp=>temp.Persons,null as List<Person>).Create();
            CountryResponse countryResponseFromAddCountry = country.ToCountryResponse();

            //mocking GetCountryByCountryID()
            _countriesRepositoryMock.Setup(
                temp => temp.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            //Act
            CountryResponse? countryResponseFromGetCountry = await _countriesGetterService.GetCountryByCountryID(country.CountryID);

            //Assert
            countryResponseFromGetCountry.Should().Be(countryResponseFromAddCountry);
        }
        #endregion
    }
}
