using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
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

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        public CounriesServiceTest()
		{
            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
		}

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request);
            });
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

            _countriesRepositoryMock
                .Setup(t => t.AddCountry(It.IsAny<Country>()))
                .ReturnsAsync(request1.ToCountry());

            _countriesRepositoryMock
              .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
              .ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesService.AddCountry(request1);

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                _countriesRepositoryMock
                    .Setup(t => t.AddCountry(It.IsAny<Country>()))
                    .ReturnsAsync(request1.ToCountry());

                _countriesRepositoryMock
                  .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
                  .ReturnsAsync(request1.ToCountry());

                await _countriesService.AddCountry(request2);

            });
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? country_request = new CountryAddRequest() { CountryName = "Japan" };

            Country country = country_request.ToCountry();

            CountryResponse country_response = country.ToCountryResponse();

            //Act

            _countriesRepositoryMock
              .Setup(t => t.AddCountry(It.IsAny<Country>()))
              .ReturnsAsync(country);

            _countriesRepositoryMock
              .Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>()))
              .ReturnsAsync(null as Country);

            CountryResponse response = await _countriesService.AddCountry(country_request);

            country_response.CountryID = response.CountryID;

            response.Should().Be(country_response);

        }

        #endregion


        #region GetAllCountries

        public async Task<List<Country>> GetCountries()
        {
            List<Country> countries = new List<Country>
    {
        new Country
        {
            CountryID = Guid.NewGuid(),
            CountryName = "USA",
            Persons = null
        },
        new Country
        {
            CountryID = Guid.NewGuid(),
            CountryName = "Canada",
            Persons = null
        },
        new Country
        {
            CountryID = Guid.NewGuid(),
            CountryName = "Mexico",
            Persons = null
        }
    };

            return await Task.FromResult(countries);
        }



        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GetAllCountries_ToBeEmptyList()
        {
            List<Country> countries = new List<Country>();

            _countriesRepositoryMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(countries);

            //Act
            List<CountryResponse> actual_country_response_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }


        [Fact]
        public async Task GetAllCountries_ShouldHaveFewCountries()
        {
            //Arrange
            List<Country> countries = await GetCountries();

            //Act
            List<CountryResponse> expected_country_response = new List<CountryResponse>();

            foreach (var item in countries)
            {
                expected_country_response.Add(item.ToCountryResponse());
            }

            _countriesRepositoryMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(countries);

            List<CountryResponse> actual_country_response = await _countriesService.GetAllCountries();

            //foreach (var item in expected_country_response)
            //{
            //    Assert.Contains(item, actual_country_response);
            //}

            // next way:

            actual_country_response.Should().BeEquivalentTo(expected_country_response);
        }
        #endregion


        #region GetCountryByCountryID

        [Fact]
        //If we supply null as CountryID, it should return null as CountryResponse
        public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
        {
            //Act
            _countriesRepositoryMock
                .Setup(x => x.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(null as Country);

            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(null);

            //Assert
            Assert.Null(country_response_from_get_method);
        }


        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
        {
            //Arrange
            Country country = new Country() { CountryID = new Guid(), CountryName = "USA", Persons = null };

            CountryResponse countryResponse = country.ToCountryResponse();

            //Act
            _countriesRepositoryMock
                .Setup(x => x.GetCountryByCountryID(It.IsAny<Guid>()))
                .ReturnsAsync(country);

            CountryResponse? country_response_from_get_method = await _countriesService.GetCountryByCountryID(country.CountryID);

            //Assert

            country_response_from_get_method.Should().BeEquivalentTo(countryResponse);
        }
        #endregion
    }
}


