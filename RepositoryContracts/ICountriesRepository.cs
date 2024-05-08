using Entities;
using System.Diagnostics.Metrics;

namespace RepositoryContracts
{

    /// <summary>
    ///   Represents data access logic for managin Person entity
    /// </summary>

    public interface ICountriesRepository
    {
        Task<Country> AddCountry(Country country);

        Task<List<Country>> GetAllCountries();

        Task<Country?> GetCountryByCountryID(Guid countryID);

        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
