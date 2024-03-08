using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
	/// <summary>
	/// Acts as a DTO for inserting a new person
	/// </summary>
	public class PersonUpdateRequest
	{
		[Required(ErrorMessage = "Person ID can't be blank")]
		public Guid PersonID { get; set; }
		public string? PersonName { get; set; }

		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Email value should be a valid email")]
		public string? Email { get; set; }

		public DateTime? DateOfBirth { get; set; }
		public GenderOptions? Gender { get; set; }
		public Guid? CountryID { get; set; }
		public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }

		/// <summary>
		/// Converts the current object of PersonUpdateRequest into a new object of Person type
		/// </summary>
		/// <returns></returns>
		public Person ToPerson()
		{
			return new Person() {PersonID = PersonID, PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), Address = Address, CountryID = CountryID, ReceiveNewsLetters = ReceiveNewsLetters };
		}
	}
}