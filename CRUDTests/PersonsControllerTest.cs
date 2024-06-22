using AutoFixture;
using CRUDExample.Controllers;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsAdderService _personsAdderService;
        private readonly IPersonsDeleterService _personsDeleterService;
        private readonly IPersonsGetterService _personsGetterService;
        private readonly IPersonsSorterService _personsSorterService;
        private readonly IPersonsUpdaterService _personsUpdaterService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsAdderService> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterService> _personsDeleterServiceMock;
        private readonly Mock<IPersonsGetterService> _personsGetterServiceMock;
        private readonly Mock<IPersonsSorterService> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterService> _personsUpdaterServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsAdderServiceMock = new Mock<IPersonsAdderService>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterService>();
            _personsGetterServiceMock = new Mock<IPersonsGetterService>();
            _personsSorterServiceMock = new Mock<IPersonsSorterService>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterService>();

            _countriesService = _countriesServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            _personsGetterServiceMock
             .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_response_list);

            _personsSorterServiceMock
             .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
             .ReturnsAsync(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<SortOrderOptions>()
            );

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }

        #endregion

        #region Create

        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            PersonAddRequest personAddRequest = _fixture.Create<PersonAddRequest>();

            Person person = personAddRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsAdderServiceMock
                .Setup(t => t.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Create(personAddRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Create_IfModelErrors_ToReturnCreateView()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
             .With(temp => temp.PersonName, null as string)
             .Create();

            Person person = personAddRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsAdderServiceMock
                .Setup(t => t.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            personsController.ModelState.AddModelError("PersonName", "Person name cannot be blank");

            IActionResult result = await personsController.Create(personAddRequest);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            viewResult.ViewData.Model.Should().Be(personAddRequest);
        }

        #endregion

        #region Edit

        [Fact]
        public async void Edit_POST_IfModelErrors_ToReturnEditView()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
             .With(temp => temp.PersonName, null as string)
             .Create();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _personsGetterServiceMock
                .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _personsUpdaterServiceMock
                .Setup(x => x.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personResponse);

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            personsController.ModelState.AddModelError("PersonName", "Person name cannot be blank");

            IActionResult result = await personsController.Edit(personUpdateRequest);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Edit_POST_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _personsGetterServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(personResponse);

            _personsUpdaterServiceMock
                .Setup(x => x.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personResponse);

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Edit(personUpdateRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async void Edit_GET_PersonExists_ToReturnEditView()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsGetterServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Edit(personUpdateRequest.PersonID);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonUpdateRequest>();
        }

        [Fact]
        public async void Edit_GET_PersonDoesNotExist_RedirectsToIndex()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            _personsGetterServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Edit(personUpdateRequest.PersonID);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion

        #region Delete

        [Fact]
        public async void Delete_POST_PersonDoesNotExist_RedirectsToIndex()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            _personsGetterServiceMock
              .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
              .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Delete(personUpdateRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async void Delete_POST_PersonExist_RedirectsToIndex()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            _personsGetterServiceMock
              .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
              .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Delete(personUpdateRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async void Delete_Get_PersonDoesNotExist_RedirectsToIndex()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            _personsGetterServiceMock
              .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
              .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Delete(personUpdateRequest.PersonID);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async void Delete_Get_PersonExist_ToReturnDeleteView()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            _personsGetterServiceMock
              .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
              .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(
                _personsGetterService,
                _personsAdderService,
                _personsUpdaterService,
                _personsDeleterService,
                _personsSorterService,
                _countriesService
            );

            //Act
            IActionResult result = await personsController.Delete(personUpdateRequest.PersonID);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<PersonResponse>();
            viewResult.ViewData.Model.Should().Be(personResponse);
        }

        #endregion
    }
}
