using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Inserting Country entity.
    /// </summary>
    public interface ICountriesAdderService
    {
        /// <summary>
        /// Adds a Country Object to the list of Countries.
        /// </summary>
        /// <param name="countryAddRequest">Country object to be added.</param>
        /// <returns>Returns Country Object after adding it (including newly generated Country Id)</returns>
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
    }
}