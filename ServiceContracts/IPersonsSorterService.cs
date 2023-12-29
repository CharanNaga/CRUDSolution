using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Sorting Person entity.
    /// </summary>
    public interface IPersonsSorterService
    {
        /// <summary>
        /// Returns Sorted List of Persons
        /// </summary>
        /// <param name="allPersons">List of Persons to sort</param>
        /// <param name="sortBy">Name of the property (key) based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC OR DESC</param>
        /// <returns>Returns sorted persons as PersonResponse List</returns>
        Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons,string sortBy, SortOrderOptions sortOrder);
    }
}
