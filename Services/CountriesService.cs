using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
	public class CountriesService : ICountriesService
	{
        private readonly PersonsDbContext _db;

        //constructor
        public CountriesService(PersonsDbContext personsDbContext)
        {
            _db = personsDbContext;
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
		{

			//Validation: countryAddRequest parameter can't be null
			if (countryAddRequest == null)
			{
				throw new ArgumentNullException(nameof(countryAddRequest));
			}

			//Validation: CountryName can't be null
			if (countryAddRequest.CountryName == null)
			{
				throw new ArgumentException(nameof(countryAddRequest.CountryName));
			}

			//Validation: CountryName can't be duplicate
			if (_db.Countries.Count(temp => temp.CountryName == countryAddRequest.CountryName) > 0)
			{
				throw new ArgumentException("Given country name already exists");
			}

			//Convert object from CountryAddRequest to Country type
			Country country = countryAddRequest.ToCountry();

			//generate CountryID
			country.CountryID = Guid.NewGuid();

			//Add country object into _countries
			_db.Add(country);
			_db.SaveChanges();

			return country.ToCountryResponse();
		}

		public List<CountryResponse> GetAllCountries()
		{
			return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
		}

		public CountryResponse? GetCountryByCountryID(Guid? countryId)
		{
			if (countryId == null)
				return null;

			var result = _db.Countries.FirstOrDefault(x => x.CountryID == countryId);

			if (result == null)
			{
				return null;
			}

			return result.ToCountryResponse();
		}
	}
}