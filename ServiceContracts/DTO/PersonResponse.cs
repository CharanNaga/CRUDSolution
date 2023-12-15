using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class that is used as return type for most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }
        /// <summary>
        /// Compares current object data with the parameter object.
        /// </summary>
        /// <param name="obj">PersonResponse object to compare</param>
        /// <returns>True/False indicating whether all person details are matched with specified parameter object.</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null) 
                return false;

            if(obj.GetType() != typeof(PersonResponse)) 
                return false; 

            PersonResponse personResponse = (PersonResponse)obj;
            return  PersonID == personResponse.PersonID && 
                    PersonName == personResponse.PersonName &&
                    Email == personResponse.Email && 
                    DateOfBirth == personResponse.DateOfBirth && 
                    Gender == personResponse.Gender && 
                    CountryID == personResponse.CountryID &&
                    Address == personResponse.Address && 
                    ReceiveNewsLetters == personResponse.ReceiveNewsLetters;
        }
        public override int GetHashCode() //we can use objects of PersonResponse as keys in dictionary.
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person Id: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date Of Birth: {DateOfBirth?.ToString("dd MMM yyyy")}, Gender: {Gender}, Country ID: {CountryID}, Address: {Address}, Receive News Letters: {ReceiveNewsLetters}, Age:{Age??0}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
    //To Convert object of Person type to PersonResponse type, we create extension method.
    //For retrieving list of persons & by default data store stores list of persons as objects of Person class.
    //So for returning list of persons, we convert each object from Person type into PersonResponse type.

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert Person class object into PersonResponse class object.
        /// </summary>
        /// <param name="person">Person Object to convert</param>
        /// <returns>Returns converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) 
                    ? Math.Floor(((DateTime.Now - person.DateOfBirth.Value).TotalDays + 1)/ 365.25)
                    : null,
                Country = person.Country?.CountryName
            };
        }
    }
}
