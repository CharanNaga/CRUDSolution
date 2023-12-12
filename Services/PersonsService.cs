using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        private readonly List<Person> _persons;
        private readonly ICountriesService _countriesService;

        //while invoking this Service in Tests, we don't want to initialize mock data and while invoking in Controllers, we want to add mock data.
        //so taking a bool variable "initialize" and setting false in Tests and setting as true at other places.
        public PersonsService(bool initialize = true)
        {
            _persons = new List<Person>();
            _countriesService = new CountriesService();
            if(initialize)
            {
                //Added mock data using mockaroo website
                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("1D163731-4D22-43B3-AD2B-37CFF2A30BD0"),
                    PersonName = "Adrien",
                    Email = "asissens0@geocities.com",
                    DateOfBirth = DateTime.Parse("1991-10-21"),
                    Gender = "Male",
                    Address = "59870 Walton Pass",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E2F8A875-0573-4D3D-AC5B-42A3160BD363")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("5A527B11-9ABB-4C40-AD94-7B182AA96EB7"),
                    PersonName = "Manon",
                    Email = "mskate1@umich.edu",
                    DateOfBirth = DateTime.Parse("1995-06-09"),
                    Gender = "Female",
                    Address = "0217 Westerfield Avenue",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("064504EA-B4F8-4658-B1FD-B214F5EB5BC7")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("44EFFA99-6BCC-4C13-8774-E2EF1BE6FB5B"),
                    PersonName = "Therese",
                    Email = "tpawelski2@nasa.gov",
                    DateOfBirth = DateTime.Parse("1996-05-20"),
                    Gender = "Female",
                    Address = "5 Debra Crossing",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("335D73C1-E274-4406-A737-E65E8C33881F")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("908C7B7A-F9D2-40FF-AD86-DB8F61BBE044"),
                    PersonName = "Duffy",
                    Email = "dmcnuff3@gravatar.com",
                    DateOfBirth = DateTime.Parse("1995-05-11"),
                    Gender = "Male",
                    Address = "7 Thierer Lane",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("E1CC350D-3293-40B3-9B3A-679B90ACB48F")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("A16A8536-BA4E-415A-989E-876911BA1079"),
                    PersonName = "Egor",
                    Email = "ewalduck4@cbslocal.com",
                    DateOfBirth = DateTime.Parse("1996-06-14"),
                    Gender = "Male",
                    Address = "08748 Birchwood Road",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E35E212B-D6BF-4B43-AB19-EA1587E9B4BF")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("14E2D400-A3F9-427A-AA12-3316CF52B2B1"),
                    PersonName = "Reginauld",
                    Email = "rwilsee5@unblog.fr",
                    DateOfBirth = DateTime.Parse("1991-07-18"),
                    Gender = "Male",
                    Address = "47 Miller Park",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("E2F8A875-0573-4D3D-AC5B-42A3160BD363")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("512D5D5A-5346-4960-8201-BC17FF827E9E"),
                    PersonName = "Celestina",
                    Email = "cwarby6@mayoclinic.com",
                    DateOfBirth = DateTime.Parse("1996-05-14"),
                    Gender = "Female",
                    Address = "24 Algoma Way",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("064504EA-B4F8-4658-B1FD-B214F5EB5BC7")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("D99A6448-8C9B-4448-B2D6-1C5AA38FCF1D"),
                    PersonName = "Anne",
                    Email = "ahick7@stanford.edu",
                    DateOfBirth = DateTime.Parse("1998-09-22"),
                    Gender = "Female",
                    Address = "05 Daystar Court",
                    ReceiveNewsLetters = false,
                    CountryID = Guid.Parse("335D73C1-E274-4406-A737-E65E8C33881F")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("0F23EC13-524D-4542-8B0D-1C34C0158805"),
                    PersonName = "Granthem",
                    Email = "gbunhill8@ibm.com",
                    DateOfBirth = DateTime.Parse("1994-04-26"),
                    Gender = "Male",
                    Address = "98644 Cordelia Plaza",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("E1CC350D-3293-40B3-9B3A-679B90ACB48F")
                });

                _persons.Add(new Person()
                {
                    PersonID = Guid.Parse("0D5F63E6-B1D2-47E8-9596-916E89F7D7DE"),
                    PersonName = "Dedra",
                    Email = "dtinker9@networkadvertising.org",
                    DateOfBirth = DateTime.Parse("1999-12-02"),
                    Gender = "Female",
                    Address = "21 Cambridge Trail",
                    ReceiveNewsLetters = true,
                    CountryID = Guid.Parse("E35E212B-D6BF-4B43-AB19-EA1587E9B4BF")
                });
            }
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
            //Converts all persons from "Person" type to "PersonResponse" type.
            //Return all PersonResponse Objects.
            return _persons.Select(p => p.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            //1. Check personID != null
            if (personID == null)
                return null;

            //2. Get matching person from List<Person> based on personID
            Person? personsFromList = _persons.FirstOrDefault(p=>p.PersonID == personID);

            //3. Convert matching person object from Person to PersonResponse type
            //4. Return PersonResponse Object
            if(personsFromList == null)
                return null;
            return personsFromList.ToPersonResponse();
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
            Person? matchingPerson = _persons.FirstOrDefault(p=>p.PersonID == personUpdateRequest.PersonID);

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


            //6. Convert the Person object to PersonResponse object
            //7. Return PersonResponse object with updated details
            return matchingPerson.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            //1. Check if personID != null
            if(personID == null)
                throw new ArgumentNullException(nameof(personID));

            //2. Get matching person object from List<Person> based on personID
            Person? matchingPerson = _persons.FirstOrDefault(p => p.PersonID == personID);

            //3. Check if matching person object is not null
            if (matchingPerson == null)
                return false;

            //4. Delete matching person object from List<Person>
            _persons.RemoveAll(p=>p.PersonID==personID);

            //5. Return boolean value indicating person object was deleted or not
            return true;
        }
    }
}