using System;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using ServiceContracts.Enums;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using System.Globalization;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
	public class PersonsSorterService : IPersonsSorterService
    {
        private readonly IPersonsRepository _personsRepository;
        
        public PersonsSorterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task <List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
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
    }
}