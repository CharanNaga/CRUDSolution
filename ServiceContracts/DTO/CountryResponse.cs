﻿using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class that is used as return type for most of the ContriesService methods.
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryID { get; set; }
        public string? CountryName { get; set; }
        //As per business logic, there is a need for converting Country object into country response.
    }
    //creating extension method for converting the country obj into country response obj.
    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country) //without modifying the Country Class definition, we are adding this method to that definition with the help of extension method.
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName
            };
        }
    }
}
