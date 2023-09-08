using AutoFixture;
using Moq;
using ServiceContracts;
using PersonManagement.Controllers;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;

namespace PersonManagementTest
{
    public class PersonControllerTest
    {
        private readonly IPersonServices _personServices;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonServices> _mockPeopleService;
        private readonly Mock<ICountriesService> _mockCountriesService;

        private readonly IFixture _fixture;

        public PersonControllerTest()
        {
            _fixture = new Fixture();

            _mockCountriesService = new Mock<ICountriesService>();
            _mockPeopleService = new Mock<IPersonServices>();

            _countriesService = _mockCountriesService.Object;
            _personServices = _mockPeopleService.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list =
                _fixture.Create<List<PersonResponse>>();

            PersonsController personsController = new PersonsController(_personServices, _countriesService);

            _mockPeopleService.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(persons_response_list);

            _mockPeopleService.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>()
                    , It.IsAny<string>()
                    , It.IsAny<SortOrderOptions>()))
                .Returns(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>()
                                    , _fixture.Create<string>()
                                    , _fixture.Create<string>()
                                    , _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IEnumerable<PersonResponse>>(viewResult.ViewData.Model);
            Assert.Equal(viewResult.ViewData.Model, persons_response_list);
        }
        #endregion

        #region Create HTTPPost
        [Fact]
        public async Task Create_ModelStateIsInvalid_ToReturnCreateView()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            PersonsController personsController = new PersonsController(_personServices, _countriesService);

            _mockCountriesService.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _mockPeopleService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            //Act
            personsController.ModelState.AddModelError("PersonName", "Person Name can not be black");
            IActionResult result = await personsController
                .Create(person_add_request);

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<PersonAddRequest>(viewResult.ViewData.Model);
            Assert.Equal(viewResult.ViewData.Model, person_add_request);
        }

        [Fact]
        public async Task Create_ModelStateIsvalid_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            PersonsController personsController = new PersonsController(_personServices, _countriesService);

            _mockCountriesService.Setup(temp => temp.GetAllCountries())
                .ReturnsAsync(countries);

            _mockPeopleService.Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
                .ReturnsAsync(person_response);

            //Act
            IActionResult redirectResult = await personsController
                .Create(person_add_request);

            //Assert
            RedirectToActionResult viewResult = Assert.IsType<RedirectToActionResult>(redirectResult);
            Assert.Equal("Index", viewResult.ActionName);
        }
        #endregion
    }
}
