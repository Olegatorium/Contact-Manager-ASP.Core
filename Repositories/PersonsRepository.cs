using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Add(person);
            await _db.SaveChangesAsync();

            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personID)
        {
            var person = await _db.Persons.Include("Country").FirstOrDefaultAsync(x => x.PersonID == personID);

            if (person == null) 
                return false;

            _db.Remove(person);
            await _db.SaveChangesAsync();

            return true;

        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();

        }

        public async Task<Person?> GetPersonByPersonID(Guid personID)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(x=>x.PersonID == personID);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? foundPersons = await _db.Persons.FirstOrDefaultAsync(x=>x.PersonID == person.PersonID);

            if (foundPersons == null)
                return person;

            foundPersons.PersonName = person.PersonName;
            foundPersons.Email = person.Email;
            foundPersons.DateOfBirth = person.DateOfBirth;
            foundPersons.Gender = person.Gender;
            foundPersons.CountryID = person.CountryID;
            foundPersons.Address = person.Address;
            foundPersons.ReceiveNewsLetters = person.ReceiveNewsLetters;
            foundPersons.TIN = person.TIN;

            await _db.SaveChangesAsync();

            return foundPersons;
        }
    }
}
