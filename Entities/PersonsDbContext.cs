﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Entities
{
    public class PersonsDbContext : DbContext
    {
        public PersonsDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country>? countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            if (countries != null)
            {
                foreach (Country country in countries)
                {
                    modelBuilder.Entity<Country>().HasData(country);
                }
            }
            //Seed to persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
            if (persons != null)
            {
                foreach (Person person in persons)
                {
                    modelBuilder.Entity<Person>().HasData(person);
                }
            }

            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");
            //Make TIN unique
            //modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN)
            //    .IsUnique();

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxNumber]) = 8");
        }

        public List<Person> Sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int Sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @Name" +
                ", @Email, @DateOfBirth, @Gender, @CountryID, @Address," +
                "@ReceiveNewsLetters", parameters);
        }
    }
}
