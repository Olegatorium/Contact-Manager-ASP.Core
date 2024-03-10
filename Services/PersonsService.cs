using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using ServiceContracts.Enums;

namespace Services
{
	public class PersonsService : IPersonsService
	{
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        //constructor
        public PersonsService(PersonsDbContext personDbContext, ICountriesService countriesService)
        {
            _db = personDbContext;
            _countriesService = countriesService;
        }

        private PersonResponse ConvertPersonToPersonResponse(Person person)
		{
			PersonResponse personResponse = person.ToPersonResponse();
			personResponse.Country = _countriesService.GetCountryByCountryID(person.CountryID)?.CountryName;
			return personResponse;
		}

		public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
		{
			//check if PersonAddRequest is not null
			if (personAddRequest == null)
			{
				throw new ArgumentNullException(nameof(personAddRequest));
			}

			//Model validation
			ValidationHelper.ModelValidation(personAddRequest);

			//convert personAddRequest into Person type
			Person person = personAddRequest.ToPerson();

			//generate PersonID
			person.PersonID = Guid.NewGuid();

			//add person object to persons list
			//_db.Add(person);
			//_db.SaveChanges();

			_db.sp_InsertPerson(person);

			//convert the Person object into PersonResponse type
			return ConvertPersonToPersonResponse(person);
		}


		public List<PersonResponse> GetAllPersons()
		{
            //return _db.Persons.ToList().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();

            return _db.sp_GetAllPersons().Select(temp => ConvertPersonToPersonResponse(temp)).ToList();
        }


		public PersonResponse? GetPersonByPersonID(Guid? personID)
		{
			if (personID == null)
				return null;

			Person? person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
			if (person == null)
				return null;

			return ConvertPersonToPersonResponse(person);

        }

		public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
		{
			List<PersonResponse> allPersons = GetAllPersons();
			List<PersonResponse> matchingPersons = allPersons;

			if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
				return matchingPersons;

			switch (searchBy)
			{
				case nameof(PersonResponse.PersonName):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.PersonName) ?
					temp.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				case nameof(PersonResponse.Email):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Email) ?
					temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;


				case nameof(PersonResponse.DateOfBirth):
					matchingPersons = allPersons.Where(temp =>
					(temp.DateOfBirth != null) ?
					temp.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
					break;

				case nameof(PersonResponse.Gender):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Gender) ?
					temp.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				case nameof(PersonResponse.CountryID):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Country) ?
					temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				case nameof(PersonResponse.Address):
					matchingPersons = allPersons.Where(temp =>
					(!string.IsNullOrEmpty(temp.Address) ?
					temp.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true)).ToList();
					break;

				default: matchingPersons = allPersons; break;
			}
			return matchingPersons;
		}

		public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
		{
			if (sortBy == nameof(PersonResponse.PersonName))
			{
				if (sortOrder == SortOrderOptions.DESC)
				{
					return allPersons = allPersons.OrderByDescending(x => x.PersonName).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.PersonName).ToList();
				}
			}
			else if (sortBy == nameof(PersonResponse.Email))
			{
                if (sortOrder == SortOrderOptions.DESC) 
				{
					return allPersons = allPersons.OrderByDescending(x => x.Email).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.Email).ToList();
				}
					
			}
			else if (sortBy == nameof(PersonResponse.DateOfBirth))
			{
				if (sortOrder == SortOrderOptions.DESC)
				{
					return allPersons = allPersons.OrderByDescending(x => x.DateOfBirth).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.DateOfBirth).ToList();
				}
			}
			else if (sortBy == nameof(PersonResponse.Gender))
			{
				if (sortOrder == SortOrderOptions.DESC)
				{
					return allPersons = allPersons.OrderByDescending(x => x.Gender).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.Gender).ToList();
				}
					
			}
			else if (sortBy == nameof(PersonResponse.Age))
			{

				if (sortOrder == SortOrderOptions.DESC) 
				{
					return allPersons = allPersons.OrderByDescending(x => x.Age).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.Age).ToList();
				}
					
			}
			else if (sortBy == nameof(PersonResponse.Address))
			{

				if (sortOrder == SortOrderOptions.DESC) 
				{
					return allPersons = allPersons.OrderByDescending(x => x.Address).ToList();
				}
				else
				{
					return allPersons = allPersons.OrderBy(x => x.Address).ToList();
				}
				
			}
            else if (sortBy == nameof(PersonResponse.Country))
            {

                if (sortOrder == SortOrderOptions.DESC)
                {
                    return allPersons = allPersons.OrderByDescending(x => x.Country).ToList();
                }
                else
                {
                    return allPersons = allPersons.OrderBy(x => x.Country).ToList();
                }

            }
            else if (sortBy == nameof(PersonResponse.ReceiveNewsLetters))
            {
                if (sortOrder == SortOrderOptions.DESC)
                {
                    return allPersons = allPersons.OrderByDescending(x => x.ReceiveNewsLetters).ToList();
                }
                else
                {
                    return allPersons = allPersons.OrderBy(x => x.ReceiveNewsLetters).ToList();
                }

            }
            else
			{
				return allPersons;
			}
		}

		public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			if (personUpdateRequest == null)
				throw new ArgumentNullException(nameof(Person));
			
			//validation
			ValidationHelper.ModelValidation(personUpdateRequest);

			// get matching person object to update
			Person? personToUpdate = _db.Persons.FirstOrDefault(x => x.PersonID == personUpdateRequest.PersonID);


			if (personToUpdate == null)
				throw new ArgumentNullException("given id person does not exist");
			else
			{
				personToUpdate.PersonName = personUpdateRequest.PersonName;
				personToUpdate.Address = personUpdateRequest.Address;
				personToUpdate.Gender = personUpdateRequest.Gender.ToString();
				personToUpdate.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
				personToUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
				personToUpdate.Email = personUpdateRequest.Email;
				personToUpdate.CountryID = personUpdateRequest.CountryID;

                _db.SaveChanges(); //Update
            }

			return ConvertPersonToPersonResponse(personToUpdate);
		}

		public bool DeletePerson(Guid? PersonId)
		{
			var foundPerson = _db.Persons.FirstOrDefault(x => x.PersonID == PersonId);

			if (foundPerson == null)
				return false;
			else
			{
				_db.Remove(foundPerson);

				_db.SaveChanges();

				return true;
			}
			 
		}
	}
}