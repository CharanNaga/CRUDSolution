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

        //constructor
        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
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
    }
}
