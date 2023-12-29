using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;

namespace Services
{
    public class PersonsAdderService : IPersonsAdderService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsAdderService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsAdderService(IPersonsRepository personsRepository,ILogger<PersonsAdderService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            //1. Check personAddRequest != null
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            //2. Validate all properties of personAddRequest
            ValidationHelper.ModelValidation(personAddRequest); //validating all properties using Model validations by calling a reusable method.

            //3. Convert personAddRequest to Person type
            Person person = personAddRequest.ToPerson();

            //4. Generate a new PersonID
            person.PersonID = Guid.NewGuid();

            //5. Then add it to the List<Person>
            await _personsRepository.AddPerson(person);

            //_db.sp_InsertPerson(person); //performing insertion using stored procedure

            //6. Return PersonResponse object with generated PersonID.
            //return ConvertPersonToPersonResponse(person);  
            return person.ToPersonResponse();
        }

    }
}