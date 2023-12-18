using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private field
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture; //TO work with autofixture

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(); //creating obj of Fixture class to generate fake data.

            //Creating empty persons list to test.
            var initialCountriesList = new List<Country>() { };
            var initialPersonsList = new List<Person>() { };

            //Mocking the ApplicationDbContext into a Mocked Dbcontext
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>().Options
                    );

            //Making use of Mocked DbContext as an application dbcontext, so that it doesn't involve interacting with the files or databases. (isolation constraint of tests)
            var dbContext = dbContextMock.Object;

            //creating MockedDbSet for the Countries & Persons Table with the empty seeded lists
            dbContextMock.CreateDbSetMock(temp => temp.Countries, initialCountriesList);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, initialPersonsList);

            //_countriesService = new CountriesService(
            //    new ApplicationDbContext(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //        ));
            //_personsService = new PersonsService(
            //     new ApplicationDbContext(
            //        new DbContextOptionsBuilder<ApplicationDbContext>().Options
            //        ),_countriesService);

            _countriesService = new CountriesService(dbContext);
            _personsService = new PersonsService(dbContext,_countriesService);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson
        //Three criteria..
        //1. When PersonAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => {
                //Act
                await _personsService.AddPerson(request);
            });
        }

        //2. When PersonName is null, throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            //PersonAddRequest request = new PersonAddRequest() { PersonName = null };
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.PersonName, null as string)
                .Create();
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _personsService.AddPerson(request);
            });
        }

        //3. When Proper person details are provided, insert into the existing list of persons and it should return an object of PersonResponse class with newly generated PersonID.
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            //PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>(); //create method initializes all properties with some default values

            //build method doesn't create object directly, instead of assigning default values to properties directly, it will check for properties with model validations. If found, then ovveride defined property with a given value in With() & then create.
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()  
                .With(temp=>temp.Email,"fakemail@example.com")
                .Create();

            //Act
            PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
            List<PersonResponse> personResponseList = await _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, personResponseList);
        }
        #endregion

        #region GetAllPersons
        //1. Without adding any person, list should be empty. List of persons should be empty before adding any persons.
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act 
            List<PersonResponse> personsResponseList = await _personsService.GetAllPersons();

            //Assert
            Assert.Empty(personsResponseList);
        }
        //2. If we add few persons, then same persons should be returned.
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test1@example.com")
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test2@example.com")
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
               .With(temp => temp.Email, "test3@example.com")
               .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 };

            //Act
            List<PersonResponse> personsListFromAddPerson = new List<PersonResponse>();
            foreach (PersonAddRequest personRequest in personAddRequests) //as AddPerson return type is PersonResponse. We initialize an empty list of PersonResponse type and will add the persons from request into the response list.
            {
                personsListFromAddPerson.Add(await _personsService.AddPerson(personRequest));
            }
            //print personListFromAddPerson
            _testOutputHelper.WriteLine("Expected: ");
            foreach(PersonResponse personResponse in personsListFromAddPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonsListFromAddPerson = await _personsService.GetAllPersons();

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromAddPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //read each element from personsListFromAddPerson
            foreach (PersonResponse expectedPerson in personsListFromAddPerson)
            {
                //Assert
                Assert.Contains(expectedPerson, actualPersonsListFromAddPerson);
            }
        }
        #endregion

        #region GetPersonByPersonID
        //1. If PersonID supplied is null, return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(personResponse);
        }

        //2. If valid PersonID supplied, return matching Person Details as PersonResponse Object.
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID()
        {
            //Arrange

            //add person object first, then search the person by given id.
            //before then, add country first. Use CountryID from the CountryResponse and provide the same while initializing CountryID property in PersonAddRequest.

            //CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"someone@example.com")
                .Create();
            PersonResponse personResponseFromAddPerson = await _personsService.AddPerson(personAddRequest);

            //Act
            PersonResponse? personResponseFromGetPerson =  await _personsService.GetPersonByPersonID(personResponseFromAddPerson.PersonID);

            //Assert
            Assert.Equal(personResponseFromAddPerson, personResponseFromGetPerson);
        }
        #endregion

        #region GetFilteredPersons

        //1. If empty string is provided as an argument and search by person name, return all persons.
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test1@example.com")
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "test2@example.com")
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 };

            //Act
            List<PersonResponse> personsListFromAddPerson = new List<PersonResponse>();
            foreach (PersonAddRequest personRequest in personAddRequests) //as AddPerson return type is PersonResponse. We initialize an empty list of PersonResponse type and will add the persons from request into the response list.
            {
                personsListFromAddPerson.Add(await _personsService.AddPerson(personRequest));
            }
            //print personListFromAddPerson
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonsListFromGetFilteredPerson = await _personsService.GetFilteredPersons(nameof(Person.PersonName),"");

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetFilteredPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //read each element from personsListFromAddPerson
            foreach (PersonResponse expectedPerson in personsListFromAddPerson)
            {
                //Assert
                Assert.Contains(expectedPerson, actualPersonsListFromGetFilteredPerson);
            }
        }

        //2. If text string is provided as an argument and search by person name, return matching persons.
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Charan")
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2 ,personAddRequest3};

            //Act
            List<PersonResponse> personsListFromAddPerson = new List<PersonResponse>();
            foreach (PersonAddRequest personRequest in personAddRequests) //as AddPerson return type is PersonResponse. We initialize an empty list of PersonResponse type and will add the persons from request into the response list.
            {
                personsListFromAddPerson.Add(await _personsService.AddPerson(personRequest));
            }
            //print personListFromAddPerson
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonsListFromGetFilteredPerson = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ha");

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetFilteredPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //read each element from personsListFromAddPerson
            foreach (PersonResponse expectedPerson in personsListFromAddPerson)
            {
                if(expectedPerson.PersonName!=null)
                {
                    if (expectedPerson.PersonName.Contains("ha", StringComparison.OrdinalIgnoreCase))
                    {
                        //Assert
                        Assert.Contains(expectedPerson, actualPersonsListFromGetFilteredPerson);
                    }
                }
            }
        }
        #endregion

        #region GetSortedPersons
        //1. when we sort based on PersonName in DESC, return persons list in DESC order on PersonName.
        [Fact]
        public async Task GetSortedPersons()
        {
            CountryAddRequest countryAddRequest1 = _fixture.Create<CountryAddRequest>();
            CountryAddRequest countryAddRequest2 = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.CountryID, countryResponse1.CountryID)
                .Create();

            PersonAddRequest personAddRequest2 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Charan")
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            PersonAddRequest personAddRequest3 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Mary")
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.CountryID, countryResponse2.CountryID)
                .Create();

            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>() { personAddRequest1, personAddRequest2, personAddRequest3 };

            //Act
            List<PersonResponse> personsListFromAddPerson = new List<PersonResponse>();
            foreach (PersonAddRequest personRequest in personAddRequests) //as AddPerson return type is PersonResponse. We initialize an empty list of PersonResponse type and will add the persons from request into the response list.
            {
                personsListFromAddPerson.Add(await _personsService.AddPerson(personRequest));
            }
            //print personListFromAddPerson
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            List<PersonResponse> personsResponseFromGetAllPersons = await _personsService.GetAllPersons();

            List<PersonResponse> actualPersonsListFromGetSortedPerson = await _personsService.GetSortedPersons(personsResponseFromGetAllPersons,nameof(Person.PersonName),SortOrderOptions.DESC);

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetSortedPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            personsListFromAddPerson = personsListFromAddPerson.OrderByDescending(p=>p.PersonName).ToList();

            //Assert
            for(int i = 0; i < personsListFromAddPerson.Count; i++)
            {
                Assert.Equal(personsListFromAddPerson[i], actualPersonsListFromGetSortedPerson[i]);
            }
        }
        #endregion

        #region UpdatePerson
        //1. When PersonUpdateRequest is null, throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async() => {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //2. If PersonID is invalid, throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() => {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //3. When PersonName is null, throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();
            
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();

            PersonResponse personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() => {
                //Act
                await _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //4. When Proper Person Details are given, update the PersonResponse Object
        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

            PersonUpdateRequest personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "William";
            personUpdateRequest.Email = "william@gmail.com";

            //Act
            PersonResponse personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);
            PersonResponse? personResponseFromGet = await _personsService.GetPersonByPersonID(personUpdateRequest.PersonID);

            //Assert
            Assert.Equal(personResponseFromGet,personResponseFromUpdate);
        }
        #endregion

        #region DeletePerson
        //1. When valid PersonID provided, return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            CountryAddRequest countryAddRequest = _fixture.Create<CountryAddRequest>();

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.CountryID, countryResponse.CountryID)
                .Create();
            PersonResponse personResponseFromAdd = await _personsService.AddPerson(personAddRequest);

            //Act
            bool isDeleted = await _personsService.DeletePerson(personResponseFromAdd.PersonID);
            
            //Assert 
            Assert.True(isDeleted);
        }

        //2. When invalid PersonID provided, return false
        [Fact]
        public async Task DeletePerson_InValidPersonID()
        {
            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert 
            Assert.False(isDeleted);
        }
        #endregion
    }
}