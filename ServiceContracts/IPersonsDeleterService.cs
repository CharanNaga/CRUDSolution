namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Deleting Person entity.
    /// </summary>
    public interface IPersonsDeleterService
    {
        /// <summary>
        /// Deletes a person based on given personID
        /// </summary>
        /// <param name="personID">PersonID to delete</param>
        /// <returns>True (If successful deletion), otherwise False</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
