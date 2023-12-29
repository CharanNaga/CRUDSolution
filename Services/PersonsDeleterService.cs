using Entities;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using Serilog;
using ServiceContracts;

namespace Services
{
    public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsDeleterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsDeleterService(IPersonsRepository personsRepository,ILogger<PersonsDeleterService> logger,IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }
        public async Task<bool> DeletePerson(Guid? personID)
        {
            //1. Check if personID != null
            if (personID == null)
                throw new ArgumentNullException(nameof(personID));

            //2. Get matching person object from List<Person> based on personID
            Person? matchingPerson = await _personsRepository.GetPersonByPersonID(personID.Value);

            //3. Check if matching person object is not null
            if (matchingPerson == null)
                return false;

            //4. Delete matching person object from List<Person>
            await _personsRepository.DeletePersonByPersonID(personID.Value);

            //5. Return boolean value indicating person object was deleted or not
            return true;
        }
    }
}