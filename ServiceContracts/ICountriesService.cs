using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating Country entity.
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a Country Object to the list of Countries.
        /// </summary>
        /// <param name="countryAddRequest">Country object to be added.</param>
        /// <returns>Returns Country Object after adding it (including newly generated Country Id)</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries from the list.
        /// </summary>
        /// <returns>All countries from the list as List of CountryResponse</returns>
        List<CountryResponse> GetAllCountries();
    }
}