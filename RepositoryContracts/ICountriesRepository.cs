using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts
{
    /// <summary>
    /// Contains Data Access Logic for managing Country Entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds new country object to the datastore
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns country object after adding to the datastore</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries from the datastore
        /// </summary>
        /// <returns>All countries from the database table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns country object based on the countryID, otherwise null
        /// </summary>
        /// <param name="countryID">CountryID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryID(Guid countryID);

        /// <summary>
        /// Returns country object based on countryName, otherwise null
        /// </summary>
        /// <param name="countryName">CountryName to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByCountryName(string countryName);

    }
}