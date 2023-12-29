using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Getting Person entity.
    /// </summary>
    public interface IPersonsGetterService
    {
       
        /// <summary>
        /// Returns all Persons
        /// </summary>
        /// <returns>Returns a list of objects of PersonResponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Returns PersonResponse object based on personID
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Matching person as a PersonResponse object</returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search value
        /// </summary>
        /// <param name="searchBy">Field to search</param>
        /// <param name="searchString">Value to search</param>
        /// <returns>All matching persons based on given search field and value</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);


        /// <summary>
        /// Returns Persons as CSV
        /// </summary>
        /// <returns>Returns memory stream with csv data of persons</returns>
        Task<MemoryStream> GetPersonsCSV();

        /// <summary>
        /// Returns Persons as Excel
        /// </summary>
        /// <returns>Returns memory stream with excel data of persons</returns>
        Task<MemoryStream> GetPersonsExcel();
    }
}
