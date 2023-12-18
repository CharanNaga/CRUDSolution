﻿using Entities;
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

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
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
            PersonAddRequest request = new PersonAddRequest() { PersonName = null };
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
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Sample Name",
                Email = "sample@gmail.com",
                DateOfBirth = DateTime.Parse("2001-01-01"),
                Gender = GenderOptions.Male,
                CountryID = Guid.NewGuid(),
                Address = "sample address",
                ReceiveNewsLetters = true
            };

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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "China" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Sample Name1",
                Email = "sample1@gmail.com",
                DateOfBirth = DateTime.Parse("2000-02-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse1.CountryID,
                Address = "sample1 address",
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Sample Name2",
                Email = "sample2@gmail.com",
                DateOfBirth = DateTime.Parse("2001-02-03"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "sample2 address",
                ReceiveNewsLetters = false
            };

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
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Sample Name",
                Email = "samplemail@gmail.com",
                DateOfBirth = DateTime.Parse("2001-01-01"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse.CountryID,
                Address = "sample email address",
                ReceiveNewsLetters = false
            };
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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "China" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Sample Name1",
                Email = "sample1@gmail.com",
                DateOfBirth = DateTime.Parse("2000-02-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse1.CountryID,
                Address = "sample1 address",
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Sample Name2",
                Email = "sample2@gmail.com",
                DateOfBirth = DateTime.Parse("2001-02-03"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "sample2 address",
                ReceiveNewsLetters = false
            };

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
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "China" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Charan",
                Email = "sample1@gmail.com",
                DateOfBirth = DateTime.Parse("2000-02-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse1.CountryID,
                Address = "sample1 address",
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Harish",
                Email = "sample2@gmail.com",
                DateOfBirth = DateTime.Parse("2001-02-03"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse2.CountryID,
                Address = "sample2 address",
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "sample3@gmail.com",
                DateOfBirth = DateTime.Parse("2002-04-01"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "sample3 address",
                ReceiveNewsLetters = true
            };

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
            //Arrange
            CountryAddRequest countryAddRequest1 = new CountryAddRequest() { CountryName = "India" };
            CountryAddRequest countryAddRequest2 = new CountryAddRequest() { CountryName = "China" };

            CountryResponse countryResponse1 = await _countriesService.AddCountry(countryAddRequest1);
            CountryResponse countryResponse2 = await _countriesService.AddCountry(countryAddRequest2);

            PersonAddRequest personAddRequest1 = new PersonAddRequest()
            {
                PersonName = "Charan",
                Email = "sample1@gmail.com",
                DateOfBirth = DateTime.Parse("2000-02-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse1.CountryID,
                Address = "sample1 address",
                ReceiveNewsLetters = true
            };
            PersonAddRequest personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Harish",
                Email = "sample2@gmail.com",
                DateOfBirth = DateTime.Parse("2001-02-03"),
                Gender = GenderOptions.Male,
                CountryID = countryResponse2.CountryID,
                Address = "sample2 address",
                ReceiveNewsLetters = false
            };
            PersonAddRequest personAddRequest3 = new PersonAddRequest()
            {
                PersonName = "Mary",
                Email = "sample3@gmail.com",
                DateOfBirth = DateTime.Parse("2002-04-01"),
                Gender = GenderOptions.Female,
                CountryID = countryResponse2.CountryID,
                Address = "sample3 address",
                ReceiveNewsLetters = true
            };

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
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid()};

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
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK"};
            CountryResponse countryResponseFromAdd= await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() 
            { 
                PersonName = "Peter", 
                CountryID = countryResponseFromAdd.CountryID,
                Email = "peter@example.com",
                Address = "England",
                Gender = GenderOptions.Male
            };
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
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "UK" };
            CountryResponse countryResponseFromAdd = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest() 
            {
                PersonName = "Peter", 
                Email = "peter@gmail.com",
                DateOfBirth = DateTime.Parse("2001-04-01"),
                Gender = GenderOptions.Male,
                CountryID = countryResponseFromAdd.CountryID,
                Address = "Peter street",
                ReceiveNewsLetters = true
            };
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
            //Arrange
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponseFromAdd = await _countriesService.AddCountry(countryAddRequest);

            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                PersonName = "Parker",
                Email = "parker@example.com",
                DateOfBirth = DateTime.Parse("2001-04-01"),
                CountryID = countryResponseFromAdd.CountryID,
                Gender = GenderOptions.Male,
                Address = "parker street",
                ReceiveNewsLetters = true
            };
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