using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly List<Country> _countries;

        //while invoking this Service in Tests, we don't want to initialize mock data and while invoking in Controllers, we want to add mock data.
        //so taking a bool variable "initialize" and setting false in Tests and setting as true at other places.
        public CountriesService(bool initialize=true) 
        {
            _countries = new List<Country>();
            if (initialize)
            {
                _countries.AddRange(
                    new List<Country>()
                    {
                        new Country()
                        {
                            CountryID = Guid.Parse("E2F8A875-0573-4D3D-AC5B-42A3160BD363"),
                            CountryName = "USA"
                        },

                        new Country()
                        {
                            CountryID = Guid.Parse("064504EA-B4F8-4658-B1FD-B214F5EB5BC7"),
                            CountryName = "Canada"
                        },

                        new Country()
                        {
                            CountryID = Guid.Parse("335D73C1-E274-4406-A737-E65E8C33881F"),
                            CountryName = "UK"
                        },

                        new Country()
                        {
                            CountryID = Guid.Parse("E1CC350D-3293-40B3-9B3A-679B90ACB48F"),
                            CountryName = "India"
                        },

                        new Country()
                        {
                            CountryID = Guid.Parse("E35E212B-D6BF-4B43-AB19-EA1587E9B4BF"),
                            CountryName = "Australia"
                        }
                    });
            }
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
            if (_countries.Where(c=>c.CountryName == countryAddRequest.CountryName).Count() > 0)
                throw new ArgumentException("Country Name already existed");

            //3. Convert countryAddRequest from CountryAddRequest type to Country type.
            Country country = countryAddRequest.ToCountry(); //Entities(Domain Models) should hide from Controller or Unit test classes.

            //4. Generate new CountryID
            country.CountryID = Guid.NewGuid();

            //5. Add to List<Country>
            _countries.Add(country);

            //6. Return CountryResponse object with generated CountryID.
            return country.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            //Converts all Countries from "Country" type to "CountryResponse" type.
            //Return all CountryResponse Objects.
            return _countries.Select(c => c.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            //1. Check countryID != null
            if (countryID == null)
                return null;

            //2. Get Matching Country from List<Country> based countryID
            Country? countryFromList = _countries.FirstOrDefault(c=>c.CountryID == countryID);


            //3. Convert matching country object from Country to CountryResponse
            //4. Return CountryResponse object
            if (countryFromList == null)
                return null;
            
            return countryFromList.ToCountryResponse();
        }
    }
}