using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
	public class CounriesServiceTest
	{
		private readonly ICountriesService _countriesService;

		public CounriesServiceTest()
		{
			_countriesService = new CountriesService(false);
		}

		#region AddCountry

		//When CountryAddRequest is null, it should throw ArgumentNullException
		[Fact]
		public void AddCountry_NullCountry()
		{
			//Arrange

			CountryAddRequest request = null;

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act
				_countriesService.AddCountry(request);
			});
		}

		//When the CountryName is null, it should throw ArgumentException
		[Fact]
		public void AddCountry_CountryNameIsNull()
		{
			//Arrange

			CountryAddRequest request = new CountryAddRequest();

			request.CountryName = null;

			//Assert
			Assert.Throws<ArgumentException>(() =>
			{
				//Act
				_countriesService.AddCountry(request);
			});
		}

		//When the CountryName is duplicate, it should throw ArgumentException
		[Fact]
		public void AddCountry_DuplicateCountryName()
		{
			//Arrange
			CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

			//Assert
			Assert.Throws<ArgumentException>(() =>
			{
				//Act
				_countriesService.AddCountry(request1);
				_countriesService.AddCountry(request2);
			});
		}


		//When you supply proper country name, it should insert (add) the country to the existing list of countries
		[Fact]
		public void AddCountry_ProperCountryDetails()
		{
			//Arrange
			CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };

			//Act
			CountryResponse response = _countriesService.AddCountry(request);

			List<CountryResponse> countries_from_GetAllCountries = _countriesService.GetAllCountries();

			//Assert
			Assert.True(response.CountryID != Guid.Empty);
			Assert.Contains(response, countries_from_GetAllCountries);
		}

		#endregion


		#region GetAllCountries

		[Fact]
		public void GetAllCountries_EmptyList()
		{
			//Act
			List<CountryResponse> countries = _countriesService.GetAllCountries();

			//Assert
			Assert.Empty(countries);
		}

		[Fact]
		public void GetAllCountries_AddFewCountries()
		{

			//Arrange
			List<CountryAddRequest> country_request_list = new List<CountryAddRequest>() {

				new CountryAddRequest() { CountryName = "USA" },
				new CountryAddRequest() { CountryName = "UK" }
			};

			//Act
			List<CountryResponse> countries_list_from_add_country = new List<CountryResponse>();

			foreach (CountryAddRequest country_request in country_request_list)
			{
				countries_list_from_add_country.Add(_countriesService.AddCountry(country_request));
			}

			List<CountryResponse> actualCountryResponseList = _countriesService.GetAllCountries();

			//read each element from countries_list_from_add_country
			foreach (CountryResponse expected_country in countries_list_from_add_country)
			{
				Assert.Contains(expected_country, actualCountryResponseList);
			}

		}

		#endregion


		#region GetCountryByCounryID

		[Fact]
		public void GetCountryByCounryID_Null_CountryId()
		{
			Guid? countryID = null;

			CountryResponse? countryResponse =_countriesService.GetCountryByCountryID(countryID);

			Assert.Null(countryResponse);	
		}




		[Fact]
		public void GetCountryByCountryID_ValidCountryID()
		{
			//Arrange

			CountryAddRequest? countryAddRequest = new CountryAddRequest()
			{
				CountryName = "Japan"
			};

			CountryResponse? country_response_from_add_request = _countriesService.AddCountry(countryAddRequest);

			Guid? countryID = country_response_from_add_request.CountryID;

			CountryResponse? country_response_from_get_id = _countriesService.GetCountryByCountryID(countryID);

			Assert.Equal(country_response_from_get_id, country_response_from_add_request);

		}

		#endregion
	}
}


