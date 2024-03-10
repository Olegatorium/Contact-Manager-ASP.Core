using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
	/// <summary>
	/// Domain module for Country
	/// </summary>
	public class Country
	{
		[Key]
        public Guid CountryID { get; set; }
		public string? CountryName { get; set; }
	}
}