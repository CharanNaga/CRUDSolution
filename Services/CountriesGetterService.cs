using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesGetterService : ICountriesGetterService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesGetterService(ICountriesRepository countriesRepository) 
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            //Converts all Countries from "Country" type to "CountryResponse" type.
            //Return all CountryResponse Objects.
            var countries = await _countriesRepository.GetAllCountries();
            return countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            //1. Check countryID != null
            if (countryID == null)
                return null;

            //2. Get Matching Country from List<Country> based countryID
            Country? countryFromList = await _countriesRepository.GetCountryByCountryID(countryID.Value);


            //3. Convert matching country object from Country to CountryResponse
            //4. Return CountryResponse object
            if (countryFromList == null)
                return null;
            
            return countryFromList.ToCountryResponse();
        }
    }
}