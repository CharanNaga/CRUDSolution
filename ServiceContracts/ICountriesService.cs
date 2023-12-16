using Microsoft.AspNetCore.Http;
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
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

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

        /// <summary>
        /// Uploads countries from excel file into Database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}