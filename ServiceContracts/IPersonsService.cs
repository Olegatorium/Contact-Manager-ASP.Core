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
		PersonResponse AddPerson(PersonAddRequest? personAddRequest);

		/// <summary>
		/// return list of all persons
		/// </summary>
		/// <returns></returns>
		List<PersonResponse> GetAllPersons();

		/// <summary>
		/// Get Person By Person ID
		/// </summary>
		/// <param name="personID"></param>
		/// <returns></returns>
		PersonResponse? GetPersonByPersonID(Guid? personID);

		List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);

		List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

		PersonResponse UpdatePerson(PersonUpdateRequest? personUpdateRequest);

		bool DeletePerson(Guid? PersonId);
	}
}
