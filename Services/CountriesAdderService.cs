using Entities;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesAdderService : ICountriesAdderService
    {
        private readonly ICountriesRepository _countriesRepository;
        public CountriesAdderService(ICountriesRepository countriesRepository) 
        {
            _countriesRepository = countriesRepository;
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
            if (await _countriesRepository.GetCountryByCountryName(countryAddRequest.CountryName) != null)
                throw new ArgumentException("Country Name already existed");

            //3. Convert countryAddRequest from CountryAddRequest type to Country type.
            Country country = countryAddRequest.ToCountry(); //Entities(Domain Models) should hide from Controller or Unit test classes.

            //4. Generate new CountryID
            country.CountryID = Guid.NewGuid();

            //5. Add to List<Country>
            await _countriesRepository.AddCountry(country);

            //6. Return CountryResponse object with generated CountryID.
            return country.ToCountryResponse();
        }
    }
}