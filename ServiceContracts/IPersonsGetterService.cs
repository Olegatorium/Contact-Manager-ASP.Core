using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface IPersonsGetterService
	{
        Task<List<PersonResponse>> GetAllPersons();

        Task<PersonResponse?> GetPersonByPersonID(Guid? personID);

        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        Task<MemoryStream> GetPersonsCSV();

        Task<MemoryStream> GetPersonsExcel();
    }
}
