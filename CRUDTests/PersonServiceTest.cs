using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonServices _personServices;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _personServices = new PersonService();
            _countriesService = new CountriesService();
            _outputHelper = testOutputHelper;
        }


        #region AddPerson
        [Fact]
        public void AddPerson_NullPersonRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
                _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public void AddPerson_NullPersonName()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                Name = null
            };

            Assert.Throws<ArgumentException>(() =>
                _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public void AddPerson_ValidPerson()
        {
            //Arrange
            PersonAddRequest personAddRequest = new PersonAddRequest()
            {
                Name = "Viet",
                Email = "Email@gmail.com",
                DateOfBirth = DateTime.Parse("2003/03/21"),
                CountryID = Guid.NewGuid(),
                Address = "VN",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };

            //Act
            var person_add_response = _personServices.AddPerson(personAddRequest);
            var list_actual_person = _personServices.GetAllPersons();
            //print person_add_response
            _outputHelper.WriteLine($"Expected: {person_add_response}");
            //print the list are added
            _outputHelper.WriteLine($"Actual: {list_actual_person.FirstOrDefault()}");
            //Assert
            Assert.True(person_add_response.PersonID != Guid.Empty);
            Assert.Contains(person_add_response, list_actual_person);
        }
        #endregion

        #region GetAllPerson
        [Fact]
        public void GetAllPerson_EmptyList()
        {
            var person_list = _personServices.GetAllPersons();

            Assert.Empty(person_list);
        }

        [Fact]
        public void GetAllPerson_ValidList()
        {
            //Arrange
            CountryAddRequest new_country = new CountryAddRequest()
            {
                CountryName = "Viet Nam"
            };

            var country_response = _countriesService.AddCountry(new_country);
            //Act
            List<PersonAddRequest> personAddRequests = new List<PersonAddRequest>()
            {
                new PersonAddRequest()
                {
                    Name = "Viet",
                    Email = "Email@gmail.com",
                    DateOfBirth = DateTime.Parse("2003/03/21"),
                    CountryID = country_response.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Male,
                    ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                    Name = "Khoi",
                    Email = "Email@gmail.com",
                    DateOfBirth = DateTime.Parse("2003/03/21"),
                    CountryID = country_response.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Female,
                    ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                    Name = "Phong",
                    Email = "Email@gmail.com",
                    DateOfBirth = DateTime.Parse("2003/03/21"),
                    CountryID = country_response.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Male,
                    ReceiveNewsLetters = true
                }
            };
            List<PersonResponse> personResponses = new List<PersonResponse>();
            foreach (PersonAddRequest person in personAddRequests)
            {
                personResponses.Add(_personServices.AddPerson(person));
            }
            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }
            //print th actual_person_list
            var actual_person_list = _personServices.GetAllPersons();
            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (var expected_person in personResponses)
            {
                Assert.Contains(expected_person, actual_person_list);
            }
        }
        #endregion

        #region GetPersonByID
        [Fact]
        public void GetPersonByID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Acts
            var person_response = _personServices.GetPersonByID(personID);
            //Assert
            Assert.Null(person_response);
        }

        [Fact]
        public void GetPersonByID_ValidPersonID()
        {
            //Arrange
            CountryAddRequest new_country = new CountryAddRequest()
            {
                CountryName = "Viet Nam"
            };

            var country_response = _countriesService.AddCountry(new_country);
            //Act
            PersonAddRequest new_person_add_request = new PersonAddRequest()
            {
                Name = "Viet",
                Email = "Email@gmail.com",
                DateOfBirth = DateTime.Parse("2003/03/21"),
                CountryID = country_response.CountryID,
                Address = "VN",
                Gender = GenderOptions.Male,
                ReceiveNewsLetters = true
            };
            var new_person_add_resopnse = _personServices.AddPerson(new_person_add_request);

            var person_response = _personServices.GetPersonByID(new_person_add_resopnse.PersonID);
            //Assert
            Assert.NotNull(person_response);
        }
        #endregion
    }
}
