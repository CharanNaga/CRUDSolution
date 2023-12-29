using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Adding Person entity.
    /// </summary>
    public interface IPersonsAdderService
    {
        /// <summary>
        /// Adds new person into list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns same person details along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
    }
}
