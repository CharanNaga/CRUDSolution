using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Class for adding a new Person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name can't be blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage ="Email can't be blank")]
        [EmailAddress(ErrorMessage ="Email Address should be a valid one")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage ="Please Select Gender of the Person")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage ="Please Select a Country")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts Current Object of PersonAddRequest into a new object of Person Type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
