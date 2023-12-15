using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _db;
        public CountriesService(PersonsDbContext db) 
        {
            _db = db;
        }
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //1. Check whether countryAddRequest != null
            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            //2. Validate all properties of countryAddRequest
            //CountryName can't be null.
            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            //CountryName can't be duplicated.
            if (_db.Countries.Count(c=>c.CountryName == countryAddRequest.CountryName) > 0)
                throw new ArgumentException("Country Name already existed");

            //3. Convert countryAddRequest from CountryAddRequest type to Country type.
            Country country = countryAddRequest.ToCountry(); //Entities(Domain Models) should hide from Controller or Unit test classes.

            //4. Generate new CountryID
            country.CountryID = Guid.NewGuid();

            //5. Add to List<Country>
            _db.Countries.Add(country);
            _db.SaveChanges();

            //6. Return CountryResponse object with generated CountryID.
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            //Converts all Countries from "Country" type to "CountryResponse" type.
            //Return all CountryResponse Objects.
            return _db.Countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            //1. Check countryID != null
            if (countryID == null)
                return null;

            //2. Get Matching Country from List<Country> based countryID
            Country? countryFromList = _db.Countries.FirstOrDefault(c=>c.CountryID == countryID);


            //3. Convert matching country object from Country to CountryResponse
            //4. Return CountryResponse object
            if (countryFromList == null)
                return null;
            
            return countryFromList.ToCountryResponse();
        }
    }
}