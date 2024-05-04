﻿using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;

namespace CRUDTests
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;
		private readonly ICountriesService _countriesService;
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly IFixture _fixture;

		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			_fixture = new Fixture();

            ///

			_testOutputHelper = testOutputHelper;

			///

			var countriesInitialData = new List<Country> { };

			DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>
				(
				new DbContextOptionsBuilder<ApplicationDbContext>().Options
				);

			ApplicationDbContext dbContext = dbContextMock.Object;

			dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

			_countriesService = new CountriesService(dbContext);

			///

			var personsInitialData = new List<Person> { };

            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _personService = new PersonsService(dbContext, _countriesService);
        }

        #region HelperMethods
        public async Task<List<PersonAddRequest>> GetListOfPersonRequests()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesService.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesService.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
               .With(temp => temp.PersonName, "Smith")
               .With(temp => temp.Email, "someone_1@example.com")
               .With(temp => temp.CountryID, country_response_1.CountryID)
               .Create();

            PersonAddRequest person_request_2 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Mary")
             .With(temp => temp.Email, "someone_2@example.com")
             .With(temp => temp.CountryID, country_response_1.CountryID)
             .Create();

            PersonAddRequest person_request_3 = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone_3@example.com")
             .With(temp => temp.CountryID, country_response_2.CountryID)
             .Create();

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            return person_requests;
        }
        #endregion

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, null as string)
             .Create();

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _personService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
			//Arrange
			PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
				.With(t => t.Email, "someone@example.com")
				.Create();
            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

            List<PersonResponse> persons_list = await _personService.GetAllPersons();

            //Assert
            Assert.True(person_response_from_add.PersonID != Guid.Empty);

            Assert.Contains(person_response_from_add, persons_list);
        }

        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(person_response_from_get);
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arange
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
                .With(t => t.Email, "someone@example.com")
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }

        // it should return all persons

        [Fact]
		public async Task GetAllPersons_CheckForElementsInResponse()
		{
            List<PersonAddRequest> person_requests = await GetListOfPersonRequests();

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			_testOutputHelper.WriteLine("Expected");

			foreach (var item in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Act
			List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

			_testOutputHelper.WriteLine("Actual:");

			foreach (var item in persons_list_from_get)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, persons_list_from_get);
			}
		}
		#endregion

		#region GetFilteredPersons

		// if search text is empty and search by is "PersonName" it should return all persons
		[Fact]
		public async Task GetFilteredPersons_EmptySearchText()
		{
            List<PersonAddRequest> person_requests = await GetListOfPersonRequests();

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			_testOutputHelper.WriteLine("Expected");

			foreach (var item in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			List<PersonResponse> filtered_persons = await _personService.GetFilteredPersons(nameof(Person.PersonName), "");

			_testOutputHelper.WriteLine("Actual:");

			foreach (var item in filtered_persons)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Assert
			foreach (PersonResponse person_response_from_add in person_response_list_from_add)
			{
				Assert.Contains(person_response_from_add, filtered_persons);
			}

		}

		//First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
		[Fact]
		public async Task GetFilteredPersons_SearchByPersonName()
		{
            List<PersonAddRequest> person_requests = await GetListOfPersonRequests();

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest item in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(item);

				person_response_list_from_add.Add(person_response);
			}

			List<PersonResponse> person_expected_response_list = new List<PersonResponse>();

			foreach (var item in person_response_list_from_add)
			{
				if (item.PersonName != null)
				{
					if (item.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
					{
						person_expected_response_list.Add(item);	
					}
				}
			}

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse item in person_expected_response_list)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Act
			List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

			//print persons_list_from_get
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse item in persons_list_from_search)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Assert
			foreach (PersonResponse item in person_expected_response_list)
			{
				if (item.PersonName != null)
				{
					if (item.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
					{
						Assert.Contains(item, persons_list_from_search);
					}
				}
			}
		}

        #endregion

		#region GetSortedPersons

		[Fact]
		public async Task GetSortedPersons()
		{

            List<PersonAddRequest> person_requests = await GetListOfPersonRequests();

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest item in person_requests)
			{
				PersonResponse person_response = await _personService.AddPerson(item);

				person_response_list_from_add.Add(person_response);
			}

			person_response_list_from_add = person_response_list_from_add.OrderByDescending(x => x.PersonName).ToList();

			//print person_response_list_from_add
			_testOutputHelper.WriteLine("Expected:");
			foreach (PersonResponse item in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Act

			List<PersonResponse> allPersons = await _personService.GetAllPersons();

			List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);


			//print persons_list_from_sort
			_testOutputHelper.WriteLine("Actual:");
			foreach (PersonResponse item in persons_list_from_sort)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Assert

			for (int i = 0; i < person_response_list_from_add.Count; i++)
			{

				Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);

			}
		}

        #endregion

        #region UpdatePerson

        // if null it must throw ArgumentNullExeption

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });
        }


        [Fact]
		public async Task UpdatePerson_PersonNameIsNull()
		{
			//Arrange

			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonName = null, Email = "oleg@gmail.com" };

			//Assert
			await Assert.ThrowsAsync<ArgumentNullException>(async() =>
			{
				//Act

				await _personService.UpdatePerson(personUpdateRequest);
			});

		}

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();

            CountryResponse country_response = await _countriesService.AddCountry(country_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, "Rahman")
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.CountryID, country_response.CountryID)
             .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_request);

            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
            Assert.Equal(person_response_from_get, person_response_from_update);


        }

        #endregion

        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);

            PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>()
                .With(t => t.Email, "someone@example.com")
                .Create();

            PersonResponse person_response_from_add = await _personService.AddPerson(person_add_request);

            //Act
            bool isDeleted = await _personService.DeletePerson(person_response_from_add.PersonID);

            //Assert
            Assert.True(isDeleted);
        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDeleted);
        }

        #endregion
    }
}