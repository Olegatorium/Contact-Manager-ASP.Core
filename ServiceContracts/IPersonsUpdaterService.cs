using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	public interface IPersonsUpdaterService
    {
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);

    }
}
