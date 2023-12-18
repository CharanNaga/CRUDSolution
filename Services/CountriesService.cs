using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _db;
        public CountriesService(ApplicationDbContext db) 
        {
            _db = db;
        }
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //1. Check whether countryAddRequest != null
            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            //2. Validate all properties of countryAddRequest
            //CountryName can't be null.
            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            //CountryName can't be duplicated.
            if (await _db.Countries.CountAsync(c=>c.CountryName == countryAddRequest.CountryName) > 0)
                throw new ArgumentException("Country Name already existed");

            //3. Convert countryAddRequest from CountryAddRequest type to Country type.
            Country country = countryAddRequest.ToCountry(); //Entities(Domain Models) should hide from Controller or Unit test classes.

            //4. Generate new CountryID
            country.CountryID = Guid.NewGuid();

            //5. Add to List<Country>
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();

            //6. Return CountryResponse object with generated CountryID.
            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            //Converts all Countries from "Country" type to "CountryResponse" type.
            //Return all CountryResponse Objects.
            return await _db.Countries.Select(c => c.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            //1. Check countryID != null
            if (countryID == null)
                return null;

            //2. Get Matching Country from List<Country> based countryID
            Country? countryFromList = await _db.Countries.FirstOrDefaultAsync(c=>c.CountryID == countryID);


            //3. Convert matching country object from Country to CountryResponse
            //4. Return CountryResponse object
            if (countryFromList == null)
                return null;
            
            return countryFromList.ToCountryResponse();
        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {

            MemoryStream memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);
            int countriesInserted = 0;

            using (ExcelPackage excelPackage =  new ExcelPackage(memoryStream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];
                int rowCount = worksheet.Dimension.Rows;
                for(int row=2; row <= rowCount; row++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[row, 1].Value);
                    if (!string.IsNullOrEmpty(cellValue))
                    {
                        string? countryName = cellValue;
                        if(_db.Countries.Where(c=>c.CountryName== countryName).Count() == 0)
                        {
                            Country country = new Country() { CountryName = countryName };
                            _db.Countries.Add(country);
                            await _db.SaveChangesAsync();
                            countriesInserted++;
                        }
                    }
                }
                return countriesInserted;
            }

        }
    }
}