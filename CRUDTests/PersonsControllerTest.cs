using AutoFixture;
using CRUDExample.Controllers;
using Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsService> _personsServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;

        private readonly Fixture _fixture;

        public PersonsControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesService>();
            _personsServiceMock = new Mock<IPersonsService>();

            _countriesService = _countriesServiceMock.Object;
            _personsService = _personsServiceMock.Object;
        }

        #region Index

        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            _personsServiceMock
             .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_response_list);

            _personsServiceMock
             .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
             .ReturnsAsync(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

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

            _personsServiceMock
                .Setup(t => t.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

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

            _personsServiceMock
                .Setup(t => t.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

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
        public async void Edit_IfModelErrors_ToReturnEditView()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Build<PersonUpdateRequest>()
             .With(temp => temp.PersonName, null as string)
             .Create();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _personsServiceMock
                .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                .ReturnsAsync(personResponse);

            _personsServiceMock
                .Setup(x => x.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personResponse);

            _countriesServiceMock
                .Setup(t => t.GetAllCountries())
                .ReturnsAsync(country_response_list);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act
            personsController.ModelState.AddModelError("PersonName", "Person name cannot be blank");

            IActionResult result = await personsController.Edit(personUpdateRequest);

            ViewResult viewResult = Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void Edit_IfNoModelErrors_ToReturnRedirectToIndex() 
        {
           PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>(); 

           Person person = personUpdateRequest.ToPerson();

           PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _personsServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(personResponse);

            _personsServiceMock
                .Setup(x => x.UpdatePerson(It.IsAny<PersonUpdateRequest>()))
                .ReturnsAsync(personResponse);

            _countriesServiceMock
             .Setup(t => t.GetAllCountries())
             .ReturnsAsync(country_response_list);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act

            IActionResult result = await personsController.Edit(personUpdateRequest);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        [Fact]
        public async void Edit_GET_PersonExists_ReturnsViewWithPersonUpdateRequest()
        {
            PersonUpdateRequest personUpdateRequest = _fixture.Create<PersonUpdateRequest>();

            Person person = personUpdateRequest.ToPerson();

            PersonResponse personResponse = person.ToPersonResponse();

            List<CountryResponse> country_response_list = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
              .Setup(t => t.GetAllCountries())
              .ReturnsAsync(country_response_list);

            _personsServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(personResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

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

            _personsServiceMock
                 .Setup(x => x.GetPersonByPersonID(It.IsAny<Guid>()))
                 .ReturnsAsync(null as PersonResponse);

            PersonsController personsController = new PersonsController(_personsService, _countriesService);

            //Act

            IActionResult result = await personsController.Edit(personUpdateRequest.PersonID);

            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
