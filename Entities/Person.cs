using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	/// <summary>
	/// Person domain model class
	/// </summary>>

	public class Person
	{
		[Key]
		public Guid PersonID { get; set; }

		[StringLength(40)]
		public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }

		public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

		public Guid? CountryID { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }
		public bool ReceiveNewsLetters { get; set; }
		public string? TIN { get; set; }

	}
}
