using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly IFixture _fixture;

        //constructor
        public CountriesServiceTest()
        {
            _fixture = new Fixture();

            //Creating empty countries list to test.
            var initialCountriesList = new List<Country>() {};

            //Mocking the ApplicationDbContext into a Mocked Dbcontext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>().Options
                    );

            //Making use of Mocked DbContext as an application dbcontext, so that it doesn't involve interacting with the files or databases. (isolation constraint of tests)
            var dbContext = dbContextMock.Object;

            //creating MockedDbSet for the Country Table with the empty seeded countries list
            dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountriesList);

            //passing the same mocked dbcontext options to the Service constructor so that services receive this mocked dbcontext, which performs dummy implementation.
            //_countriesService = new CountriesService(dbContext);
            _countriesService = new CountriesService(null);
        }
        #region AddCountry
        //Four Requirements for Test..
        //1. When CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            ////Assert
            //await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
            //    //Act
            //    await _countriesService.AddCountry(request);
            //});
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //2. When CountryName is null, throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            //CountryAddRequest? request = new CountryAddRequest() { CountryName = null};
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, null as string)
                .Create();

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //    await _countriesService.AddCountry(request);
            //});
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //3. When CountryName is Duplicate, throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            //CountryAddRequest? request1 = new CountryAddRequest() { CountryName="India"};
            //CountryAddRequest? request2 = new CountryAddRequest() { CountryName="India"};
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>()
                .With(c => c.CountryName, "India")
                .Create();

            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async() =>
            //{
            //    //Act
            //    await _countriesService.AddCountry(request1);
            //    await _countriesService.AddCountry(request2);
            //});
            Func<Task> action = async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            };
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //4. When proper CountryName is supplied, insert the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Create<CountryAddRequest>();

            //Act
            CountryResponse response = await _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = await _countriesService.GetAllCountries(); //added country should be visible in GetAllCountries()

            //Assert

            //Assert.True(response.CountryID != Guid.Empty);
            response.CountryID.Should().NotBe(Guid.Empty);

            //Assert.Contains(response, countriesFromGetAllCountries); //It compares objects with Equals() method.Equals() by default compares by reference rather than by data.
            countriesFromGetAllCountries.Should().Contain(response);
        }
        #endregion

        #region GetAllCountries
        //1. Without adding any country, list should be empty. List of countries should be empty before adding any countries.
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //Assert
            //Assert.Empty(actualCountryResponseList); //if it is empty, test case pass.
            actualCountryResponseList.Should().BeEmpty();
        }

        //2. If we add few countries, then same countries should be returned.
        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
               //new CountryAddRequest(){CountryName = "India"},
               //new CountryAddRequest(){CountryName = "Australia"}

               _fixture.Create<CountryAddRequest>(),
               _fixture.Create<CountryAddRequest>()
            };

            //Act
            List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();
            foreach(CountryAddRequest countryRequest in countryAddRequests) //as AddCountry return type is CountryResponse. We initialize an empty list of CountryResponse type and will add the countries from request into the response list.
            {
                countriesListFromAddCountry.Add(await _countriesService.AddCountry(countryRequest));
            }

            List<CountryResponse> actualContriesListFromGetCountry = await _countriesService.GetAllCountries();

            //read each element from countriesListFromAddCountry
            //foreach(CountryResponse expectedCountry in countriesListFromAddCountry)
            //{
            //    Assert.Contains(expectedCountry, actualContriesListFromGetCountry);
            //}
            actualContriesListFromGetCountry.Should().BeEquivalentTo(countriesListFromAddCountry);
        }
        #endregion

        #region GetCountryByCountryID
        //1. if CountryID supplied is null, it should return null as CountryResponse.
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryId = null;

            //Act 
            CountryResponse? countryResponseFromGetCountryById = await _countriesService.GetCountryByCountryID(countryId);

            //Assert
            //Assert.Null(countryResponseFromGetCountryById); //if null, test case pass.
            countryResponseFromGetCountryById.Should().BeNull();
        }

        //2. if valid CountryID is supplied, return matching CountryDetails as CountryResponse Object.
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            //add country object first, then search the country by given id.
            //CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Japan" };
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponseFromAddCountry = await _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponseFromGetCountry = await _countriesService.GetCountryByCountryID(countryResponseFromAddCountry.CountryID);

            //Assert
            //Assert.Equal(countryResponseFromAddCountry, countryResponseFromGetCountry);
            countryResponseFromGetCountry.Should().Be(countryResponseFromAddCountry);
        }
        #endregion
    }
}
