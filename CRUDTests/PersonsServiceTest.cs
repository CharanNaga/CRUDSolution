using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private field
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
        }

        #region AddPerson
        //Three criteria..
        //1. When PersonAddRequest is null, throw ArgumentNullException
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => {
                //Act
                _personsService.AddPerson(request);
            });
        }

        //2. When PersonName is null, throw ArgumentException
        [Fact]
        public void AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest request = new PersonAddRequest() { PersonName = null };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.AddPerson(request);
            });
        }

        //3. When Proper person details are provided, insert into the existing list of persons and it should return an object of PersonResponse class with newly generated PersonID.
        [Fact]
        public void AddPerson_ProperPersonDetails()
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
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
            List<PersonResponse> personResponseList = _personsService.GetAllPersons();

            //Assert
            Assert.True(personResponse.PersonID != Guid.Empty);
            Assert.Contains(personResponse, personResponseList);
        }
        #endregion

        #region GetPersonByPersonID
        //1. If PersonID supplied is null, return null as PersonResponse
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? personResponse = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(personResponse);
        }

        //2. If valid PersonID supplied, return matching Person Details as PersonResponse Object.
        [Fact]
        public void GetPersonByPersonID_ValidPersonID()
        {
            //Arrange

            //add person object first, then search the person by given id.
            //before then, add country first. Use CountryID from the CountryResponse and provide the same while initializing CountryID property in PersonAddRequest.
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse countryResponse = _countriesService.AddCountry(countryAddRequest);

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
            PersonResponse personResponseFromAddPerson = _personsService.AddPerson(personAddRequest);

            //Act
            PersonResponse? personResponseFromGetPerson = _personsService.GetPersonByPersonID(personResponseFromAddPerson.PersonID);

            //Assert
            Assert.Equal(personResponseFromAddPerson, personResponseFromGetPerson);
        }
        #endregion
    }
}
