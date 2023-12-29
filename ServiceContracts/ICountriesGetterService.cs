using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Retrieving Country entity.
    /// </summary>
    public interface ICountriesGetterService
    {
        /// <summary>
        /// Returns all countries from the list.
        /// </summary>
        /// <returns>All countries from the list as List of CountryResponse</returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns a CountryResponse Object based on provided CountryID
        /// </summary>
        /// <param name="countryID">CountryID to Search</param>
        /// <returns>Matching Country as CountryResponse Object</returns>
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);

    }
}