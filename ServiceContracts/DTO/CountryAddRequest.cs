using Entities;
using System;
using System.Collections.Generic;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class for adding a new Country
    /// </summary>
    public class CountryAddRequest
    {
        public string? CountryName { get; set; }
        public Country ToCountry() //By calling this method in service, we can convert an existing CountryAddRequest object into new object of Country class.
        {
            return new Country { CountryName = CountryName };
        }
    }
}
