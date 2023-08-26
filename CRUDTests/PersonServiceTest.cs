using Entities;
using Microsoft.VisualBasic;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Collections.ObjectModel;
using System.Runtime.ConstrainedExecution;
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
            _personServices = new PersonService(false);
            _countriesService = new CountriesService(false);
            _outputHelper = testOutputHelper;
        }

        public List<PersonResponse> CreatedPersons()
        {
            CountryAddRequest new_country = new CountryAddRequest()
            {
                CountryName = "Viet Nam"
            };

            var country_response = _countriesService.AddCountry(new_country);
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

            return personResponses;
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

        #region GetFilteredPersons
        [Fact]
        public void GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            List<PersonResponse> personResponses = CreatedPersons();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var actual_person_list_search = 
                _personServices.GetFilteredPersons(nameof(Person.Name),"");

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list_search)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (var expected_person in personResponses)
            {
                Assert.Contains(expected_person, actual_person_list_search);
            }
        }

        [Fact]
        public void GetFilteredPersons_AllowSearchString()
        {
            //Arrange
            List<PersonResponse> personResponses = CreatedPersons();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                if (personResponse.Name == null) continue;
                if (personResponse.Name.Contains("o", StringComparison.OrdinalIgnoreCase))
                    _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var actual_person_list_search =
                _personServices.GetFilteredPersons(nameof(Person.Name), "o");

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list_search)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (PersonResponse expected_person in personResponses)
            {
                if (expected_person.Name != null)
                {
                    if (expected_person.Name.Contains("o",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(expected_person, actual_person_list_search);
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
            List<PersonResponse> personResponses = CreatedPersons()
                .OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var person_list_sort =
                _personServices.GetSortedPersons(personResponses, nameof(Person.Name), SortOrderOptions.DESC);

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in person_list_sort)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            for (int i = 0; i < personResponses.Count; i++)
            {
                Assert.Equal(personResponses[i], person_list_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public void UpdatePerson_NullPersonRequest()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
                //Act
                _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
                //Act
                _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public void UpdatePerson_NullPersonName()
        {
            var personList = CreatedPersons();

            //Arrange
            var personUpdateRequest = personList.First().ToPersonUpdateRequest();

            personUpdateRequest.Name = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
                //Act
                _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public void UpdatePerson_ValidRequest()
        {
            //Arrange
            var personList = CreatedPersons();
            if (personList == null) return;
            //Act
            PersonUpdateRequest personUpdateRequest = personList.First().ToPersonUpdateRequest();

            personUpdateRequest.Name = "Phuong";
            personUpdateRequest.Email = "Phuong@gmail.com";

            PersonResponse personUpdateResponse = 
                _personServices.UpdetePerson(personUpdateRequest);
            //print the list
            _outputHelper.WriteLine("Before Update: ");
            foreach (var person in personList)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            //print the updeted list
            var list_after_update = _personServices.GetAllPersons();
            _outputHelper.WriteLine("After Update: ");
            foreach (var person in list_after_update) 
            {
                _outputHelper.WriteLine(person.ToString());
            }
            var person_updeted = _personServices.GetPersonByID(personUpdateRequest.PersonID);
            //Assert
            Assert.Equal(person_updeted, personUpdateResponse);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public void DeletePerson_ValidID()
        {
            //Arrange
            List<PersonResponse> people = CreatedPersons();
            var person = people.First();

            //Act
            bool isDelete = _personServices.DeletePerson(person.PersonID);

            //Assert
            Assert.True(isDelete);
        }

        [Fact]
        public void DeletePerson_InvalidID()
        {
            //Arrange
            List<PersonResponse> people = CreatedPersons();
            var person = people.First();

            //Act
            bool isDelete = _personServices.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDelete);
        }
        #endregion
    }
}
