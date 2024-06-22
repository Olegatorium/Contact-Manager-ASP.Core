using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;
using Services.PersonsServices;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            _testOutputHelper = testOutputHelper;

            _personsAdderService = new PersonsAdderService(_personsRepository);
            _personsDeleterService = new PersonsDeleterService(_personsRepository);
            _personsGetterService = new PersonsGetterService(_personsRepository);
            _personsSorterService = new PersonsSorterService(_personsRepository);
            _personsUpdaterService = new PersonsUpdaterService(_personsRepository);
        }

        #region HelperMethods
        public List<Person> GetListOfPersons()
        {
            List<Person> persons = new List<Person>()
        {
            _fixture.Build<Person>()
            .With(t=>t.Email, "someone_1@example.com")
            .With(t=>t.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(t=>t.Email, "someone_2@example.com")
            .With(t=>t.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(t=>t.Email, "someone_3@example.com")
            .With(t=>t.Country, null as Country)
            .Create()
        };

            return persons;
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
                await _personsAdderService.AddPerson(personAddRequest);
            });
        }

        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock.Setup(t => t.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

            //Assert
            Func<Task> action = async () =>
            {
                //Act
                await _personsAdderService.AddPerson(personAddRequest);
            };

            await action.Should().ThrowAsync<ArgumentException>();

            //// Second variant:
            //await Assert.ThrowsAsync<ArgumentException>(async () =>
            //{
            //    await _personsAdderService.AddPerson(personAddRequest);
            //});
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(t => t.Email, "someone@example.com")
                .Create();

            Person person = personAddRequest.ToPerson();

            PersonResponse person_response_expected = person.ToPersonResponse();

            // if we supply any argument value to the AddPerson it should return the same return value
            _personsRepositoryMock.Setup(t => t.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse person_response_from_add = await _personsAdderService.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            //Assert
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            person_response_from_add.Should().Be(person_response_expected);

            //// Second variant:
            //Assert.True(person_response_from_add.PersonID != Guid.Empty);
        }

        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(personID);

            //Assert
            person_response_from_get.Should().BeNull();

            //Second variant:
            //Assert.Null(person_response_from_get);
        }

        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID_ToBeSuccessful()
        {
            //Arange
            Person person = _fixture.Build<Person>()
                .With(t => t.Email, "someone@example.com")
                .With(t => t.Country, null as Country)
                .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(t => t.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            PersonResponse? person_response_from_get = await _personsGetterService.GetPersonByPersonID(person.PersonID);

            //Assert
            person_response_expected.Should().Be(person_response_from_get);

            //Second variant:
            //Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Arrange
            _personsRepositoryMock.Setup(t => t.GetAllPersons()).ReturnsAsync(new List<Person>());

            //Act
            List<PersonResponse> persons_from_get = await _personsGetterService.GetAllPersons();

            //Assert
            persons_from_get.Should().BeEmpty();
        }

        // it should return all persons
        [Fact]
        public async Task GetAllPersons_WithFewPersons_ToBeSuccessful()
        {
            List<Person> persons = GetListOfPersons();
            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected");

            foreach (var item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _personsRepositoryMock.Setup(t => t.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personsGetterService.GetAllPersons();

            _testOutputHelper.WriteLine("Actual:");

            foreach (var item in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion

        #region GetFilteredPersons

        // if search text is empty and search by is "PersonName" it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText_ToBeSuccessful()
        {
            List<Person> persons = GetListOfPersons();
            List<PersonResponse> person_response_list_expected = persons.Select(t => t.ToPersonResponse()).ToList();

            _testOutputHelper.WriteLine("Expected");

            foreach (var item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _personsRepositoryMock.Setup(t => t.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            List<PersonResponse> filtered_persons = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "");

            _testOutputHelper.WriteLine("Actual:");

            foreach (var item in filtered_persons)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            person_response_list_expected.Should().BeEquivalentTo(filtered_persons);
        }

        // we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>()
    {
        _fixture.Build<Person>()
        .With(t=>t.Email, "someone_1@example.com")
        .With(t=>t.Country, null as Country)
        .With(t=>t.PersonName, "marina")
        .Create(),

        _fixture.Build<Person>()
        .With(t=>t.Email, "someone_2@example.com")
        .With(t=>t.Country, null as Country)
        .Create(),

        _fixture.Build<Person>()
        .With(t=>t.Email, "someone_3@example.com")
        .With(t=>t.Country, null as Country)
        .Create()
    };

            List<PersonResponse> person_response = persons.Select(t => t.ToPersonResponse()).ToList();

            var person_response_list_expected = new List<PersonResponse>();

            foreach (var item in person_response)
            {
                if (item.PersonName != null && item.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                {
                    person_response_list_expected.Add(item);
                }
            }

            List<Person> expectedPersons = persons.Where(p => p.PersonName != null && p.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase)).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            _personsRepositoryMock.Setup(t => t.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
               .ReturnsAsync(expectedPersons);

            List<PersonResponse> persons_list_from_search = await _personsGetterService.GetFilteredPersons(nameof(Person.PersonName), "ma");

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse item in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            persons_list_from_search.Should().OnlyContain(t => t.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            List<Person> persons = GetListOfPersons();
            List<PersonResponse> person_response = persons.Select(t => t.ToPersonResponse()).ToList();
            var person_response_list_expected = person_response.OrderByDescending(x => x.PersonName).ToList();

            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse item in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            List<PersonResponse> persons_list_from_sort = await _personsSorterService.GetSortedPersons(person_response, nameof(Person.PersonName), SortOrderOptions.DESC);

            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse item in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(item.ToString());
            }

            //Assert
            person_response_list_expected.Should().BeEquivalentTo(persons_list_from_sort);
        }

        #endregion

        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Act
            Func<Task> action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            PersonUpdateRequest person_update_request = _fixture.Build<PersonUpdateRequest>().Create();

            //Act
            var action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            //Act
            var action = async () =>
            {
                await _personsUpdaterService.UpdatePerson(person_update_request);
            };

            //Assert
            await action.Should().ThrowAsync<ArgumentException>();
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                 .With(temp => temp.Country, null as Country)
                 .With(temp => temp.Gender, "Male")
                 .Create();

            PersonResponse person_response_before_update = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_before_update.ToPersonUpdateRequest();
            person_update_request.PersonName = "William";
            person_update_request.Email = "william@example.com";

            _personsRepositoryMock
              .Setup(t => t.UpdatePerson(It.IsAny<Person>()))
              .ReturnsAsync(person_update_request.ToPerson());

            _personsRepositoryMock
              .Setup(t => t.GetPersonByPersonID(It.IsAny<Guid>()))
              .ReturnsAsync(person_update_request.ToPerson());

            //Act
            PersonResponse person_response_from_update = await _personsUpdaterService.UpdatePerson(person_update_request);

            bool check = (person_response_before_update.PersonName != person_response_from_update.PersonName &&
               person_response_before_update.Email != person_response_from_update.Email &&
               person_response_from_update.PersonName == "William" &&
               person_response_from_update.Email == "william@example.com" &&
               person_response_from_update.PersonID == person_response_before_update.PersonID &&
               person_response_from_update.Age == person_response_before_update.Age &&
               person_response_from_update.Address == person_response_before_update.Address &&
               person_response_from_update.CountryID == person_response_before_update.CountryID &&
               person_response_from_update.Country == person_response_before_update.Country &&
               person_response_from_update.Gender == person_response_before_update.Gender &&
               person_response_from_update.ReceiveNewsLetters == person_response_before_update.ReceiveNewsLetters)
               ? true
               : false;

            // Assert
            check.Should().Be(true);
        }

        #endregion

        #region DeletePerson

        //If you supply a valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                 .With(temp => temp.Email, "someone@example.com")
                 .With(temp => temp.Country, null as Country)
                 .With(temp => temp.Gender, "Male")
                 .Create();

            PersonResponse person_response = person.ToPersonResponse();

            _personsRepositoryMock
                .Setup(t => t.DeletePersonByPersonId(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personsRepositoryMock
                .Setup(t => t.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(person);

            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(person_response.PersonID);

            //Assert
            isDeleted.Should().BeTrue();
        }

        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personsDeleterService.DeletePerson(Guid.NewGuid());

            //Assert
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}