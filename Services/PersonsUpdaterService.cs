using Entities;
using Exceptions;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;


namespace Services
{
    public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsUpdaterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterService(IPersonsRepository personsRepository,ILogger<PersonsUpdaterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //1. Check personUpdateRequest != null
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(Person));

            //2. Validate all properties of personUpdateRequest
            ValidationHelper.ModelValidation(personUpdateRequest);

            //3. Get matching person object from List<Person> based on PersonID
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personUpdateRequest.PersonID);

            //4. Check if matching person object is not null
            if (matchingPerson == null)
                throw new InvalidPersonIDException("Given PersonID doesn't exist");

            //5. Updates all details from PersonUpdateRequest object to Person object
            matchingPerson.PersonName = personUpdateRequest.PersonName;
            matchingPerson.Email = personUpdateRequest.Email;
            matchingPerson.DateOfBirth = personUpdateRequest.DateOfBirth;
            matchingPerson.Gender = personUpdateRequest.Gender.ToString();
            matchingPerson.CountryID = personUpdateRequest.CountryID;
            matchingPerson.Address = personUpdateRequest.Address;
            matchingPerson.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;

            await _personsRepository.UpdatePerson(matchingPerson);


            //6. Convert the Person object to PersonResponse object
            //7. Return PersonResponse object with updated details
            //return ConvertPersonToPersonResponse(matchingPerson);
            return matchingPerson.ToPersonResponse();
        }
    }
}