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
	public class PersonsAdderService : IPersonsAdderService
	{
        private readonly IPersonsRepository _personsRepository;
        
        public PersonsAdderService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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
			await _personsRepository.AddPerson(person);

			//convert the Person object into PersonResponse type
			return person.ToPersonResponse();
		}
    }
}