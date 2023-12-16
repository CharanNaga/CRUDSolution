using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Person entity.
    /// </summary>
    public interface IPersonsService
    {
        /// <summary>
        /// Adds new person into list of persons
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>Returns same person details along with newly generated PersonID</returns>
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

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
        /// Returns Sorted List of Persons
        /// </summary>
        /// <param name="allPersons">List of Persons to sort</param>
        /// <param name="sortBy">Name of the property (key) based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC OR DESC</param>
        /// <returns>Returns sorted persons as PersonResponse List</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons,string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates specified Person Details based on given personID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update including PersonID</param>
        /// <returns>PersonResponse object after updation</returns>
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Deletes a person based on given personID
        /// </summary>
        /// <param name="personID">PersonID to delete</param>
        /// <returns>True (If successful deletion), otherwise False</returns>
        Task<bool> DeletePerson(Guid? personID);

        /// <summary>
        /// Returns Persons as CSV
        /// </summary>
        /// <returns>Returns memory stream with csv data</returns>
        Task<MemoryStream> GetPersonsCSV();
    }
}
