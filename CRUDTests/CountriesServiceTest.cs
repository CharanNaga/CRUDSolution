using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System;
using System.Collections.Generic;
using Services;
using Microsoft.EntityFrameworkCore;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        //constructor
        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(
                new PersonsDbContext(
                    new DbContextOptionsBuilder<PersonsDbContext>().Options
                    ));
        }
        #region AddCountry
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
        [Fact]
        public void AddCountry_CountryNameIsNull()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null};

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request);
            });
        }

        //3. When CountryName is Duplicate, throw ArgumentException
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName="India"};
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName="India"};

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        //4. When proper CountryName is supplied, insert the country to the existing list of countries
        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Canada" };

            //Act
            CountryResponse response = _countriesService.AddCountry(request);
            List<CountryResponse> countriesFromGetAllCountries = _countriesService.GetAllCountries(); //added country should be visible in GetAllCountries()

            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countriesFromGetAllCountries); //It compares objects with Equals() method.Equals() by default compares by reference rather than by data.
        }
        #endregion

        #region GetAllCountries
        //1. Without adding any country, list should be empty. List of countries should be empty before adding any countries.
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actualCountryResponseList); //if it is empty, test case pass.
        }

        //2. If we add few countries, then same countries should be returned.
        [Fact]
        public void GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
               new CountryAddRequest(){CountryName = "India"},
               new CountryAddRequest(){CountryName = "Australia"}
            };

            //Act
            List<CountryResponse> countriesListFromAddCountry = new List<CountryResponse>();
            foreach(CountryAddRequest countryRequest in countryAddRequests) //as AddCountry return type is CountryResponse. We initialize an empty list of CountryResponse type and will add the countries from request into the response list.
            {
                countriesListFromAddCountry.Add(_countriesService.AddCountry(countryRequest));
            }

            List<CountryResponse> actualContriesListFromAddCountry = _countriesService.GetAllCountries();

            //read each element from countriesListFromAddCountry
            foreach(CountryResponse expectedCountry in countriesListFromAddCountry)
            {
                Assert.Contains(expectedCountry, actualContriesListFromAddCountry);
            }
        }
        #endregion

        #region GetCountryByCountryID
        //1. if CountryID supplied is null, it should return null as CountryResponse.
        [Fact]
        public void GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryId = null;

            //Act 
            CountryResponse? countryResponseFromGetCountryById = _countriesService.GetCountryByCountryID(countryId);

            //Assert
            Assert.Null(countryResponseFromGetCountryById); //if null, test case pass.
        }

        //2. if valid CountryID is supplied, return matching CountryDetails as CountryResponse Object.
        [Fact]
        public void GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            //add country object first, then search the country by given id.
            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = "Japan" };
            CountryResponse countryResponseFromAddCountry = _countriesService.AddCountry(countryAddRequest);

            //Act
            CountryResponse? countryResponseFromGetCountry = _countriesService.GetCountryByCountryID(countryResponseFromAddCountry.CountryID);

            //Assert
            Assert.Equal(countryResponseFromAddCountry, countryResponseFromGetCountry);
        }
        #endregion
    }
}
