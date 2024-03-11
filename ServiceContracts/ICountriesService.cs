using ServiceContracts.DTO;

namespace ServiceContracts
{
	public interface ICountriesService
	{
		Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Returns all countries
        /// </summary>
        /// <returns></returns>
        Task<List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// Returns Country object based on given country id
        /// </summary>
        /// <param name="CountryId"></param>
        /// <returns></returns>
        Task<CountryResponse>? GetCountryByCountryID(Guid? CountryId);
	}
}