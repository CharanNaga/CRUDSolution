using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Contains Data Access Logic for managing Person Entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a new person object to the datastore
        /// </summary>
        /// <param name="person">person object to add</param>
        /// <returns>Returns person object after adding to the datastore</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all persons from the datastore
        /// </summary>
        /// <returns>All Persons from the table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Return Person object based on personID, otherwise null
        /// </summary>
        /// <param name="personID">PersonID to search</param>
        /// <returns>Matching Person or null</returns>
        Task<Person?> GetPersonByPersonID(Guid personID);

        /// <summary>
        /// Returns all person objects based on given expression
        /// </summary>
        /// <param name="predicate">Linq expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Updates specified person details based on personID
        /// </summary>
        /// <param name="person">Person object to update</param>
        /// <returns>Person object after updation</returns>
        Task<Person> UpdatePerson(Person person);

        /// <summary>
        /// Deletes person object based on personID
        /// </summary>
        /// <param name="personID">PersonID to delete</param>
        /// <returns>Returns true, if deletion is successful; otherwise false</returns>
        Task<bool> DeletePersonByPersonID(Guid personID);


    }
}
