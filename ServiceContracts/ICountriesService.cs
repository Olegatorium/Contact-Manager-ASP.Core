using ServiceContracts.DTO;

namespace ServiceContracts
{
	public interface ICountriesService
	{
		CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

		/// <summary>
		/// Returns all countries
		/// </summary>
		/// <returns></returns>
		List<CountryResponse> GetAllCountries();

		/// <summary>
		/// Returns Country object based on given country id
		/// </summary>
		/// <param name="CountryId"></param>
		/// <returns></returns>
		CountryResponse? GetCountryByCountryID(Guid? CountryId);
	}
}