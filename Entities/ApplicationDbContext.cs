using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed to Countries

            string countriesJson = File.ReadAllText("countries.json");

            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (var item in countries)
            {
                modelBuilder.Entity<Country>().HasData(item);
            }

            //Seed to Persons

            string personsJson = File.ReadAllText("persons.json");

            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (var item in persons)
            {
                modelBuilder.Entity<Person>().HasData(item);
            }

            //// Fluent API

            //modelBuilder.Entity<Person>().Property(temp => temp.TIN).HasColumnName("TaxIdNumber")
            //    .HasColumnType("varchar(11)").HasDefaultValue("ABC12345");

            //modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdNumber]) = 11");
        }

      //  public List<Person> sp_GetAllPersons()
      //  {
      //      return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
      //  }

      //  public int sp_InsertPerson(Person person)
      //  {
      //      SqlParameter[] parameters = new SqlParameter[] {
      //  new SqlParameter("@PersonID", person.PersonID),
      //  new SqlParameter("@PersonName", person.PersonName),
      //  new SqlParameter("@Email", person.Email),
      //  new SqlParameter("@DateOfBirth", person.DateOfBirth),
      //  new SqlParameter("@Gender", person.Gender),
      //  new SqlParameter("@CountryID", person.CountryID),
      //  new SqlParameter("@Address", person.Address),
      //  new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters),
      //  new SqlParameter("@TaxIdNumber", person.TIN)
      //};

      //      int efectedRows = Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID, @PersonName, " +
      //          "@Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters, @TaxIdNumber", parameters);

      //      return efectedRows;
      //  }
    }
}