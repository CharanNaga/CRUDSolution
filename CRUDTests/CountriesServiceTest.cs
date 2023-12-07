using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System;
using System.Collections.Generic;
using Services;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }
        //Four Requirements for Test..
        //1. When CountryAddRequest is null, throw ArgumentNullException
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //2. When CountryName is null, throw ArgumentException

        //3. When CountryName is Duplicate, throw ArgumentException

        //4. When proper CountryName is supplied, insert the country to the existing list of countries
    }
}
