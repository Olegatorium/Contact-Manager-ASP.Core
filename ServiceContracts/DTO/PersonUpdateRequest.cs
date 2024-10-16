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

        [Required(ErrorMessage = "Person Name can't be blank")]
        [StringLength(40, ErrorMessage = "Person Name can't exceed 40 characters.")]
        public string? PersonName { get; set; }

		[Required(ErrorMessage = "Email can't be blank")]
		[EmailAddress(ErrorMessage = "Email value should be a valid email")]
        [StringLength(40, ErrorMessage = "Email can't exceed 40 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "DateOfBirth can't be blank")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender can't be blank")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "You should choose a country in the list")]
		public Guid? CountryID { get; set; }

        [StringLength(200, ErrorMessage = "Address can't exceed 200 characters.")]
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