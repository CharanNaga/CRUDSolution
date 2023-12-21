using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Domain Model Class for storing Person Details
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)] //nvarchar(40)
        public string? PersonName { get; set; }

        [StringLength(40)] //nvarchar(40) 
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)] //nvarchar(10)
        public string? Gender { get; set; }

        //UniqueIdentifier
        public Guid? CountryID { get; set; }

        [StringLength(200)] //nvarchar(200)
        public string? Address { get; set; }

        //bit
        public bool ReceiveNewsLetters { get; set; }
        public string? TIN { get; set; }
        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; }

        public override string ToString()
        {
            return $"Person ID: {PersonID}, Person Name: {PersonName}, Email: {Email}, Date of Birth: {DateOfBirth?.ToString("yyyy-MM-dd")}, Gender: {Gender}, CountryID: {CountryID}, Country: {Country?.CountryName}, Address: {Address}, Receive NewsLetters: {ReceiveNewsLetters}";
        }
    }
}
