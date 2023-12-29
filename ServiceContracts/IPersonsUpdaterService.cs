using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Updating Person entity.
    /// </summary>
    public interface IPersonsUpdaterService
    {
        /// <summary>
        /// Updates specified Person Details based on given personID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including PersonID</param>
        /// <returns>PersonResponse object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
    }
}
