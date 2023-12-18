using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options):base(options)
        {
        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set;}

        //Inorder to bind the dbsets to corresponding tables, we will override OnModelCreating()
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to countries
            string countriesJson = File.ReadAllText("countries.json");
            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJson);
            foreach(var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //Seed to persons
            string personsJson = File.ReadAllText("persons.json");
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson);
            foreach (var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }

            //Using Fluent API to set Column Name, DataType & Default Value for any property in a table
            modelBuilder.Entity<Person>().Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            //Some more settings adding to a property
            //modelBuilder.Entity<Person>().HasIndex(p => p.TIN).IsUnique();
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber])=8");

            //Table relations with Fluent API
            //modelBuilder.Entity<Person>(entity => //Every Country has set of Persons.
            //{
            //    entity.HasOne<Country>(c => c.Country) //Country property here is from Person Model Class
            //    .WithMany(p => p.Persons) //Persons Property here is from Country Model Class
            //    .HasForeignKey(p => p.PersonID);
            //});
            //Instead of doing above, simply mention [ForeignKey("CountryID")] on Country Property in Person.cs
        }
        //Creating a method to call GetAllPersons StoredProcedure
        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        //creating a method to call InsertPerson StoredProcedure
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
                new SqlParameter("@PersonID",person.PersonID),
                new SqlParameter("@PersonName",person.PersonName),
                new SqlParameter("@Email",person.Email),
                new SqlParameter("@DateOfBirth",person.DateOfBirth),
                new SqlParameter("@Gender",person.Gender),
                new SqlParameter("@CountryID",person.CountryID),
                new SqlParameter("@Address",person.Address),
                new SqlParameter("@ReceiveNewsLetters",person.ReceiveNewsLetters)
            };
            return 
                Database.ExecuteSqlRaw(
                    "EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@ReceiveNewsLetters"
                    ,sp);
        }
    }
}
