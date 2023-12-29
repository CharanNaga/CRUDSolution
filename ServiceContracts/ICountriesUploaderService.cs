using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for Uploading Country entity.
    /// </summary>
    public interface ICountriesUploaderService
    {
        /// <summary>
        /// Uploads countries from excel file into Database
        /// </summary>
        /// <param name="formFile">Excel file with list of countries</param>
        /// <returns>Returns number of countries added</returns>
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}