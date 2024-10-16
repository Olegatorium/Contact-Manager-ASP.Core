using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
	/// <summary>
	/// DTO CLASS for adding a new country
	/// </summary>
	public class CountryAddRequest
	{
        [Required(ErrorMessage = "Country can't be blank")]
        public string? CountryName { get; set; }

		public Country ToCountry()
		{
			return new Country()
			{
				CountryName = this.CountryName
			};
		}
	}
}
