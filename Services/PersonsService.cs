using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        public PersonsService()
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
        }
        //Helper method to convert Person to PersonResponse. We have every property except Country property in PersonResponse. 
        //So assigning the Country property with the CountryName property which is available from GetCountryByCountryID() from CountriesService.
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(personResponse.CountryID)?.CountryName;
            return personResponse;
        }
        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            //1. Check personAddRequest != null
            if(personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //2. Validate all properties of personAddRequest
            ValidationHelper.ModelValidation(personAddRequest); //validating all properties using Model validations by calling a reusable method.

            //3. Convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();

            //4. Generate a new PersonID
            person.PersonID = Guid.NewGuid();

            //5. Then add it to the List<Person>
            _persons.Add(person);

            //6. Return PersonResponse object with generated PersonID.
            return ConvertPersonToPersonResponse(person);  
        }

        public List<PersonResponse> GetAllPersons()
        {
            throw new NotImplementedException();
        }
    }
}
