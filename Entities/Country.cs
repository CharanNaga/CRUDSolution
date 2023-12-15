using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Domain Model for storing Country Details.
    /// </summary>
    public class Country 
    {
        //In this we will declare all the properties that we want to store as per the DB Storage.
        //we wont expose this country class to presentation layer i.e., argument to a method.
        //For that, we will use DTO's, where controller sends country details as an object of CountryAddRequest DTO so that it doesn't create and send an object of country class.
        //Service creates a protection layer surrounding the domain model.
        [Key]
        public Guid CountryID { get; set; } //By Guid, values are unlimited so app can scale up to any level. 
        public string? CountryName { get; set; }
        public virtual ICollection<Person>? Persons { get; set;} //We can access all corresponding persons based on Country class's CountryID property
    }
}