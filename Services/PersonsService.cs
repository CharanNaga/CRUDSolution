using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonsService(PersonsDbContext db, ICountriesService countriesService)
        {
            _db = db;
            _countriesService = countriesService;
        }
        //Helper method to convert Person to PersonResponse. We have every property except Country property in PersonResponse. 
        //So assigning the Country property with the CountryName property which is available from GetCountryByCountryID() from CountriesService.
        private PersonResponse ConvertPersonToPersonResponse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
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
            _db.Persons.Add(person);
            _db.SaveChanges();

            //_db.sp_InsertPerson(person); //performing insertion using stored procedure

            //6. Return PersonResponse object with generated PersonID.
            return ConvertPersonToPersonResponse(person);  
        }

        public List<PersonResponse> GetAllPersons()
        {
            var persons = _db.Persons.Include("Country").ToList(); //Using Include() to Make use of Navigation Property
            //Converts all persons from "Person" type to "PersonResponse" type.
            //Return all PersonResponse Objects.

            //return _db.Persons.ToList() //Converting linq to entities expression
            //    .Select(p => ConvertPersonToPersonResponse(p)).ToList();

            return persons //By using navigation property so that we can access CountryID and CountryName properties like persons.Country.CountryName
                .Select(p => ConvertPersonToPersonResponse(p)).ToList();

            //return _db.sp_GetAllPersons() //using stored procedures to avoid further errors
            //   .Select(p => ConvertPersonToPersonResponse(p)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            //1. Check personID != null
            if (personID == null)
                return null;

            //2. Get matching person from List<Person> based on personID
            //Person? personsFromList = _db.Persons.FirstOrDefault(p=>p.PersonID == personID);
            Person? personsFromList = _db.Persons.Include("Country").FirstOrDefault(p=>p.PersonID == personID);

            //3. Convert matching person object from Person to PersonResponse type
            //4. Return PersonResponse Object
            if(personsFromList == null)
                return null;
            return ConvertPersonToPersonResponse(personsFromList);
        }

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;

            //1. Check if searchBy != null
            if(string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
                return matchingPersons;

            //2. Get matching persons from List<Person> based on given searchBy and searchString.
            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(p=>
                    (!string.IsNullOrEmpty(p.PersonName)
                    ?p.PersonName.Contains(searchString,StringComparison.OrdinalIgnoreCase)
                    :true)).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(p =>
                    (!string.IsNullOrEmpty(p.Email)
                    ? p.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true)).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(p =>
                    (p.DateOfBirth != null)
                    ?p.DateOfBirth.Value.ToString("dd MMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(p =>
                    (!string.IsNullOrEmpty(p.Gender)
                    ? p.Gender.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true)).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(p =>
                    (!string.IsNullOrEmpty(p.Country)
                    ? p.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true)).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(p =>
                    (!string.IsNullOrEmpty(p.Address)
                    ? p.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    : true)).ToList();
                    break;

                default: 
                    matchingPersons = allPersons;
                    break;
            }
            //3. Convert matching persons from Person to PersonResponse type. (Done in switch case).
            //4. Return all matching PersonResponse objects
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if(string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.DateOfBirth).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Age).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Age).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC)
                => allPersons.OrderBy(p => p.ReceiveNewsLetters).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(p => p.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };
            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //1. Check personUpdateRequest != null
            if(personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //2. Validate all properties of personUpdateRequest
            ValidationHelper.ModelValidation(personUpdateRequest);

            //3. Get matching person object from List<Person> based on PersonID
            Person? matchingPerson = _db.Persons.FirstOrDefault(p=>p.PersonID == personUpdateRequest.PersonID);

            //4. Check if matching person object is not null
            if(matchingPerson == null)
                throw new ArgumentException("Given PersonID doesn't exist");

            //5. Updates all details from PersonUpdateRequest object to Person object
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            _db.SaveChanges();


            //6. Convert the Person object to PersonResponse object
            //7. Return PersonResponse object with updated details
            return ConvertPersonToPersonResponse(matchingPerson);
        }

        public bool DeletePerson(Guid? personID)
        {
            //1. Check if personID != null
            if(personID == null)
                throw new ArgumentNullException(nameof(personID));

            //2. Get matching person object from List<Person> based on personID
            Person? matchingPerson = _db.Persons.FirstOrDefault(p => p.PersonID == personID);

            //3. Check if matching person object is not null
            if (matchingPerson == null)
                return false;

            //4. Delete matching person object from List<Person>
            _db.Persons.Remove(_db.Persons.First(p=>p.PersonID == personID));
            _db.SaveChanges();

            //5. Return boolean value indicating person object was deleted or not
            return true;
        }
    }
}