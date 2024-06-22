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
	public class PersonsDeleterService : IPersonsDeleterService
    {
        private readonly IPersonsRepository _personsRepository;
        
        public PersonsDeleterService(IPersonsRepository personsRepository)
        {
            _personsRepository = personsRepository;
        }

		public async Task <bool> DeletePerson(Guid? personID)
		{
			if (await _personsRepository.DeletePersonByPersonId(personID.Value))
				return true;

			return false;
			 
		}
    }
}