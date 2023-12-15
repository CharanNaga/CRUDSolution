using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Entities
{
    public class PersonsDbContext:DbContext
    {
        public PersonsDbContext(DbContextOptions options):base(options)
        {
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set;}

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
        }
    }
}
