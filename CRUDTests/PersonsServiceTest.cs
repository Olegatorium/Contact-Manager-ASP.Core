﻿using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;

namespace CRUDTests
{
	public class PersonsServiceTest
	{
		private readonly IPersonsService _personService;
		private readonly ICountriesService _countriesService;
		private readonly ITestOutputHelper _testOutputHelper;

		public PersonsServiceTest(ITestOutputHelper testOutputHelper)
		{
			_personService = new PersonsService(false);
			_countriesService = new CountriesService(false);
			_testOutputHelper = testOutputHelper;
		}

		#region AddPerson

		//When we supply null value as PersonAddRequest, it should throw ArgumentNullException
		[Fact]
		public void AddPerson_NullPerson()
		{
			//Arrange
			PersonAddRequest? personAddRequest = null;

			//Act
			Assert.Throws<ArgumentNullException>(() =>
			{
				_personService.AddPerson(personAddRequest);
			});
		}


		//When we supply null value as PersonName, it should throw ArgumentException
		[Fact]
		public void AddPerson_PersonNameIsNull()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = null };

			//Act
			Assert.Throws<ArgumentException>(() =>
			{
				_personService.AddPerson(personAddRequest);
			});
		}

		//When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
		[Fact]
		public void AddPerson_ProperPersonDetails()
		{
			//Arrange
			PersonAddRequest? personAddRequest = new PersonAddRequest() { PersonName = "Person name...", Email = "person@example.com", Address = "sample address", CountryID = Guid.NewGuid(), Gender = GenderOptions.Male, DateOfBirth = DateTime.Parse("2000-01-01"), ReceiveNewsLetters = true };

			//Act
			PersonResponse person_response_from_add = _personService.AddPerson(personAddRequest);

			List<PersonResponse> persons_list = _personService.GetAllPersons();

			//Assert
			Assert.True(person_response_from_add.PersonID != Guid.Empty);

			Assert.Contains(person_response_from_add, persons_list);
		}

		#endregion

		#region GetPersonByPersonID

		//If we supply null as PersonID, it should return null as PersonResponse
		[Fact]
		public void GetPersonByPersonID_NullPersonID()
		{
			//Arrange
			Guid? personID = null;

			//Act
			PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(personID);

			//Assert
			Assert.Null(person_response_from_get);
		}


		//If we supply a valid person id, it should return the valid person details as PersonResponse object
		[Fact]
		public void GetPersonByPersonID_WithPersonID()
		{
			//Arange
			CountryAddRequest country_request = new CountryAddRequest() { CountryName = "Canada" };
			CountryResponse country_response = _countriesService.AddCountry(country_request);

			PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = GenderOptions.Male, ReceiveNewsLetters = false };

			PersonResponse person_response_from_add = _personService.AddPerson(person_request);

			PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(person_response_from_add.PersonID);

			//Assert
			Assert.Equal(person_response_from_add, person_response_from_get);
		}

		#endregion

		#region GetAllPersons

		//GetAllPersons() should return an empty list by default

		[Fact]
		public void GetAllPersons_EmptyList()
		{
			//Act

			List<PersonResponse> persons_from_get = _personService.GetAllPersons();


			//Assert

			Assert.Empty(persons_from_get);
		}

		// it should return all persons

		[Fact]
		public void GetAllPersons_CheckForElementsInResponse()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}

			_testOutputHelper.WriteLine("Expected");

			foreach (var item in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			//Act
			List<PersonResponse> persons_list_from_get = _personService.GetAllPersons();

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
		public void GetFilteredPersons_EmptySearchText()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest person_request in person_requests)
			{
				PersonResponse person_response = _personService.AddPerson(person_request);
				person_response_list_from_add.Add(person_response);
			}


			_testOutputHelper.WriteLine("Expected");

			foreach (var item in person_response_list_from_add)
			{
				_testOutputHelper.WriteLine(item.ToString());
			}

			List<PersonResponse> filtered_persons = _personService.GetFilteredPersons(nameof(Person.PersonName), "");

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
		public void GetFilteredPersons_SearchByPersonName()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest item in person_requests)
			{
				PersonResponse person_response = _personService.AddPerson(item);

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
			List<PersonResponse> persons_list_from_search = _personService.GetFilteredPersons(nameof(Person.PersonName), "ma");

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
		public void GetSortedPersons()
		{
			//Arrange
			CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
			CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

			CountryResponse country_response_1 = _countriesService.AddCountry(country_request_1);
			CountryResponse country_response_2 = _countriesService.AddCountry(country_request_2);

			PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male, Address = "address of smith", CountryID = country_response_1.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

			PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = GenderOptions.Female, Address = "address of mary", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

			PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = GenderOptions.Male, Address = "address of rahman", CountryID = country_response_2.CountryID, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

			List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

			List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

			foreach (PersonAddRequest item in person_requests)
			{
				PersonResponse person_response = _personService.AddPerson(item);

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

			List<PersonResponse> allPersons = _personService.GetAllPersons();

			List<PersonResponse> persons_list_from_sort = _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);


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

		[Fact]
		public void UpdatePerson_IsNull()
		{
			//Arrange
			PersonUpdateRequest? personUpdateRequest = null;

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act

				_personService.UpdatePerson(personUpdateRequest);
			});

		}

		[Fact]
		public void UpdatePerson_InvalidPersonId()
		{
			//Arrange

			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid(),
				Email = "mark@gmail.com"};

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act
				_personService.UpdatePerson(personUpdateRequest);
			});

		}

		[Fact]
		public void UpdatePerson_PersonNameIsNull()
		{
			//Arrange

			PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonName = null, Email = "oleg@gmail.com" };

			//Assert
			Assert.Throws<ArgumentNullException>(() =>
			{
				//Act

				_personService.UpdatePerson(personUpdateRequest);
			});

		}

		//First, add a new person and try to update the person name and email
		[Fact]
		public void UpdatePerson_PersonFullDetailsUpdation()
		{
			//Arrange
			CountryAddRequest country_add_request = new CountryAddRequest() { CountryName = "UK" };
			CountryResponse country_response_from_add = _countriesService.AddCountry(country_add_request);

			PersonAddRequest person_add_request = new PersonAddRequest() { PersonName = "John", CountryID = country_response_from_add.CountryID, Address = "Abc road", DateOfBirth = DateTime.Parse("2000-01-01"), Email = "abc@example.com", Gender = GenderOptions.Male, ReceiveNewsLetters = true };

			PersonResponse person_response_from_add = _personService.AddPerson(person_add_request);

			PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();
			person_update_request.PersonName = "William";
			person_update_request.Email = "william@example.com";

			//Act
			PersonResponse person_response_from_update = _personService.UpdatePerson(person_update_request);

			PersonResponse? person_response_from_get = _personService.GetPersonByPersonID(person_response_from_update.PersonID);

			//Assert
			Assert.Equal(person_response_from_get, person_response_from_update);

		}

		#endregion

		#region DeletePersons

		// if you supply an invalid PersonId, it should return false

		[Fact]
		public void DeletePerson_InvalidPersonId()
		{
			Guid id = Guid.NewGuid();

			var response = _personService.DeletePerson(id);

			Assert.False(response);
		}

		[Fact]
		public void DeletePerson_ValidPersonId()
		{
			CountryAddRequest country_request = new CountryAddRequest() { CountryName = "USA" };

			CountryResponse country_response = _countriesService.AddCountry(country_request);

			PersonAddRequest person_request = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = GenderOptions.Male,
				Address = "address of smith", CountryID = country_response.CountryID, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

			PersonResponse personResponse = _personService.AddPerson(person_request);

			var response = _personService.DeletePerson(personResponse.PersonID);

			Assert.True(response);

		}

		#endregion
	}
}