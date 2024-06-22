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
	public class PersonsUpdaterService : IPersonsUpdaterService
    {
        private readonly IPersonsRepository _personsRepository;
        
        public PersonsUpdaterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

		public async Task <PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
		{
			if (personUpdateRequest == null)
				throw new ArgumentNullException(nameof(Person));
			
			//validation
			ValidationHelper.ModelValidation(personUpdateRequest);

			// get matching person object to update
			Person? personToUpdate = await _personsRepository.UpdatePerson(personUpdateRequest.ToPerson());

			if (personToUpdate == null)
				throw new ArgumentNullException("given id person does not exist");

			return personToUpdate.ToPersonResponse();
		}
    }
}