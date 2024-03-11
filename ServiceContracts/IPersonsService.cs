using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface IPersonsService
	{
		/// <summary>
		/// Add new Person into the list
		/// </summary>
		/// <param name="personAddRequest"></param>
		/// <returns></returns>
		Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// return list of all persons
        /// </summary>
        /// <returns></returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Get Person By Person ID
        /// </summary>
        /// <param name="personID"></param>
        /// <returns></returns>
        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

		Task<List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

        Task<bool> DeletePerson(Guid? PersonId);

        Task<MemoryStream> GetPersonsCSV();

        Task<MemoryStream> GetPersonsExcel();
    }
}
