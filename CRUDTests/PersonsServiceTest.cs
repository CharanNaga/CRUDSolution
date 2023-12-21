using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Linq.Expressions;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private field
        #region Private readonly fields
        private readonly IPersonsService _personsService;

        private readonly IPersonsRepository _personsRepository;
        private readonly Mock<IPersonsRepository> _personsRepositoryMock;

        private readonly Mock<ILogger<PersonsService>> _loggerMock;
        private readonly ILogger<PersonsService> _logger;

        private readonly Mock<IDiagnosticContext> _diagnosticContextMock;
        private readonly IDiagnosticContext _diagnosticContext;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture; //TO work with autofixture
        #endregion

        //constructor
        #region Constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture(); //creating obj of Fixture class to generate fake data.
            _personsRepositoryMock = new Mock<IPersonsRepository>(); //to provide dummy implementation of methods, using which we can mock any methods
            _personsRepository = _personsRepositoryMock.Object;

            _loggerMock = new Mock<ILogger<PersonsService>>();
            _logger = _loggerMock.Object;

            _diagnosticContextMock = new Mock<IDiagnosticContext>();
            _diagnosticContext = _diagnosticContextMock.Object;

            _personsService = new PersonsService(_personsRepository,_logger,_diagnosticContext);
            _testOutputHelper = testOutputHelper;
        }
        #endregion

        #region AddPerson
        //Three criteria..
        //1. When PersonAddRequest is null, throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Writing Assertions using FluentAssertions
            Func<Task> action = async () =>
            {
                //Act
                await _personsService.AddPerson(request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //2. When PersonName is null, throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange            
            PersonAddRequest request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();
            Person person = request.ToPerson();
            //mocking repository
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Writing Assertions using FluentAssertions
            Func<Task> action = async () =>
            {
                //Act
                await _personsService.AddPerson(request);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //3. When Proper person details are provided, insert into the existing list of persons and it should return an object of PersonResponse class with newly generated PersonID.
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange

            //build method doesn't create object directly, instead of assigning default values to properties directly, it will check for properties with model validations. If found, then ovveride defined property with a given value in With() & then create.
            PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "fakemail@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            //If we supply any argument value to AddPerson(), it should return same return value
            //We will mock all the repository methods, which are being called as a part of corresponding service method.
            _personsRepositoryMock.Setup(
                temp => temp.AddPerson(
                    It.IsAny<Person>() //Argument Data type
                    )).ReturnsAsync(person); //Return Data type

            //Act
            PersonResponse personResponseFromAddPerson = await _personsService.AddPerson(personAddRequest);
            personResponseExpected.PersonID = personResponseFromAddPerson.PersonID;

            //Assert
            personResponseFromAddPerson.PersonID.Should().NotBe(Guid.Empty);
        }
        #endregion

        #region GetAllPersons
        //1. Without adding any person, list should be empty. List of persons should be empty before adding any persons.
        [Fact]
        public async Task GetAllPersons_EmptyList_ToBeEmptyList()
        {
            //Arrange 
            //mocking GetAllPersons()
            List<Person> persons = new List<Person>();
            _personsRepositoryMock.Setup(
                temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> personsResponseList = await _personsService.GetAllPersons();

            //Assert
            personsResponseList.Should().BeEmpty();
        }
        //2. If we add few persons, then same persons should be returned.
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personsListFromAddPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print personListFromAddPersonExpected
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPersonExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //mocking GetAllPersons() 
            _personsRepositoryMock.Setup(
                temp => temp.GetAllPersons())
                .ReturnsAsync(persons);
            //Act
            List<PersonResponse> actualPersonsListFromGetPerson = await _personsService.GetAllPersons();

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            actualPersonsListFromGetPerson.Should().BeEquivalentTo(personsListFromAddPersonExpected);
        }
        #endregion

        #region GetPersonByPersonID
        //1. If PersonID supplied is null, return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? personResponse = await _personsService.GetPersonByPersonID(personID);

            //Assert
            personResponse.Should().BeNull();
        }

        //2. If valid PersonID supplied, return matching Person Details as PersonResponse Object.
        [Fact]
        public async Task GetPersonByPersonID_ValidPersonID_ToBeSuccessful()
        {
            //Arrange

            //We should call only GetPersonByPersonID(), so removing other methods by converting return types to Person
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@example.com")
                .With(temp => temp.Country,null as Country) //removes circular reference (removes interconnection b/w person obj with country obj & vice versa) which raises due to navigation properties defined in Master & Child Model classes. 
                .Create();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            //mocking GetPersonByPersonID() as the service is calling only one repository method.
            //If we supply any guid value, it should return person object.
            _personsRepositoryMock.Setup(
                temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse? personResponseFromGetPerson = await _personsService.GetPersonByPersonID(person.PersonID);

            //Assert
            personResponseFromGetPerson.Should().Be(personResponseExpected);
        }
        #endregion

        #region GetFilteredPersons

        //1. If empty string is provided as an argument and search by person name, return all persons.
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personsListFromAddPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();       
            
            //print personListFromAddPersonExpected
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPersonExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //mocking GetFilteredPersons()
            _personsRepositoryMock.Setup(
                temp=>temp.GetFilteredPersons(
                    It.IsAny<Expression<Func<Person,bool>>>()))
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> actualPersonsListFromGetFilteredPerson = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print actualPersonsListFromAddPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetFilteredPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            actualPersonsListFromGetFilteredPerson.Should().BeEquivalentTo(personsListFromAddPersonExpected);
        }

        //2. If text string is provided as an argument and search by person name, return matching persons.
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personsListFromAddPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print personListFromAddPersonExpected
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPersonExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //mocking GetFilteredPersons()
            _personsRepositoryMock.Setup(
                temp => temp.GetFilteredPersons(
                    It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Act
            List<PersonResponse> actualPersonsListFromGetFilteredPerson = await _personsService.GetFilteredPersons(nameof(Person.PersonName), "ha");

            //print actualPersonsListFromGetFilteredPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetFilteredPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            actualPersonsListFromGetFilteredPerson.Should().BeEquivalentTo(personsListFromAddPersonExpected);
        }
        #endregion

        #region GetSortedPersons
        //1. when we sort based on PersonName in DESC, return persons list in DESC order on PersonName.
        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "test1@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                 _fixture.Build<Person>()
                .With(temp => temp.Email, "test2@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),

                  _fixture.Build<Person>()
                .With(temp => temp.Email, "test3@example.com")
                .With(temp => temp.Country, null as Country)
                .Create(),
            };

            List<PersonResponse> personsListFromAddPersonExpected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //print personListFromAddPersonExpected
            _testOutputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personsListFromAddPersonExpected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //mocking GetAllPersons()
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> personsResponseFromGetAllPersons = await _personsService.GetAllPersons();

            //no need of mocking, because we aren't calling any repository in the service for GetSortedPersons()
            List<PersonResponse> actualPersonsListFromGetSortedPerson = await _personsService.GetSortedPersons(personsResponseFromGetAllPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print actualPersonsListFromGetSortedPerson
            _testOutputHelper.WriteLine("Actual: ");
            foreach (PersonResponse personResponse in actualPersonsListFromGetSortedPerson)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            actualPersonsListFromGetSortedPerson.Should().BeInDescendingOrder(temp => temp.PersonName);
        }
        #endregion

        #region UpdatePerson
        //1. When PersonUpdateRequest is null, throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;

            //Assert
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(personUpdateRequest);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //2. If PersonID is invalid, throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
                .Create();
           //Act 
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(personUpdateRequest);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //3. When PersonName is null, throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, null as string)
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.Gender,"Male")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse personResponseFromAdd = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponseFromAdd.ToPersonUpdateRequest();
            //Act
            Func<Task> action = async () =>
            {
                await _personsService.UpdatePerson(personUpdateRequest);
            };
            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //4. When Proper Person Details are given, update the PersonResponse Object
        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetails_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.Gender, "Male")
                .With(temp => temp.Country, null as Country)
                .Create();
            PersonResponse personResponseExpected = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = personResponseExpected.ToPersonUpdateRequest();

            //mocking UpdatePerson()
            _personsRepositoryMock.Setup(
                temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //mocking GetPersonByPersonID() as all the methods that are being called in the service must be mocked before making a call.
            _personsRepositoryMock.Setup(
                temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse personResponseFromUpdate = await _personsService.UpdatePerson(personUpdateRequest);

            //Assert
            personResponseFromUpdate.Should().Be(personResponseExpected);
        }
        #endregion

        #region DeletePerson
        //1. When valid PersonID provided, return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.PersonName, "Harish")
                .With(temp => temp.Email, "test@example.com")
                .With(temp => temp.Gender, "Male")
                .With(temp => temp.Country, null as Country)
                .Create();

            //mocking DeletePersonByPersonID()
            _personsRepositoryMock.Setup(
                temp => temp.DeletePersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            //mocking GetPersonsByPersonID()
            _personsRepositoryMock.Setup(
                temp => temp.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsService.DeletePerson(person.PersonID);
            //Assert 
            isDeleted.Should().BeTrue();
        }

        //2. When invalid PersonID provided, return false
        [Fact]
        public async Task DeletePerson_InValidPersonID()
        {
            //Act
            bool isDeleted = await _personsService.DeletePerson(Guid.NewGuid());

            //Assert 
            isDeleted.Should().BeFalse();
        }
        #endregion
    }
}