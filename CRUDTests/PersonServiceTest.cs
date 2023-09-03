using Entities;
using Microsoft.EntityFrameworkCore;
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
            _countriesService = new CountriesService(new
                PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));

            _personServices = new PersonService(new 
                PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options), _countriesService);
            _outputHelper = testOutputHelper;
        }

        public async Task<List<PersonResponse>> CreatedPersons()
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
                    CountryID = country_response.Result.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Male,
                    ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                    Name = "Khoi",
                    Email = "Email@gmail.com",
                    DateOfBirth = DateTime.Parse("2003/03/21"),
                    CountryID = country_response.Result.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Female,
                    ReceiveNewsLetters = true
                },
                new PersonAddRequest()
                {
                    Name = "Phong",
                    Email = "Email@gmail.com",
                    DateOfBirth = DateTime.Parse("2003/03/21"),
                    CountryID = country_response.Result.CountryID,
                    Address = "VN",
                    Gender = GenderOptions.Male,
                    ReceiveNewsLetters = true
                }
            };

            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest person in personAddRequests)
            {
                personResponses.Add(await _personServices.AddPerson(person));
            }

            return personResponses;
        }


        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPersonRequest()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_NullPersonName()
        {
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                Name = null
            };

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_ValidPerson()
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
            var person_add_response = await _personServices.AddPerson(personAddRequest);
            var list_actual_person = await _personServices.GetAllPersons();
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
        public async Task GetAllPerson_EmptyList()
        {
            var person_list = await _personServices.GetAllPersons();

            Assert.Empty(person_list);
        }

        [Fact]
        public async Task GetAllPerson_ValidList()
        {
            //Arrange
            CountryAddRequest new_country = new CountryAddRequest()
            {
                CountryName = "Viet Nam"
            };

            var country_response = await _countriesService.AddCountry(new_country);
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
                personResponses.Add(await _personServices.AddPerson(person));
            }
            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }
            //print th actual_person_list
            var actual_person_list = await _personServices.GetAllPersons();
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
        public async Task GetPersonByID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Acts
            var person_response = await _personServices.GetPersonByID(personID);
            //Assert
            Assert.Null(person_response);
        }

        [Fact]
        public async Task GetPersonByID_ValidPersonID()
        {
            //Arrange
            CountryAddRequest new_country = new CountryAddRequest()
            {
                CountryName = "Viet Nam"
            };

            var country_response = await _countriesService.AddCountry(new_country);
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
            var new_person_add_resopnse = await _personServices.AddPerson(new_person_add_request);

            var person_response = await _personServices.GetPersonByID(new_person_add_resopnse.PersonID);
            //Assert
            Assert.NotNull(person_response);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            List<PersonResponse> personResponses = await CreatedPersons();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var actual_person_list_search = await
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
        public async Task GetFilteredPersons_AllowSearchString()
        {
            //Arrange
            List<PersonResponse> personResponses = await CreatedPersons();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (PersonResponse personResponse in personResponses)
            {
                if (personResponse.Name == null) continue;
                if (personResponse.Name.Contains("o", StringComparison.OrdinalIgnoreCase))
                    _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var actual_person_list_search = await
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
        public async Task GetSortedPersons()
        {
            //Arrange
            List<PersonResponse> personResponseList = await CreatedPersons();
            
            var personResponses = personResponseList.OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase).ToList();

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
        public async Task UpdatePerson_NullPersonRequest()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                //Act
                await _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid()
            };
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                //Act
                await _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_NullPersonName()
        {
            var personList = await CreatedPersons();

            //Arrange
            var personUpdateRequest =  personList.First().ToPersonUpdateRequest();

            personUpdateRequest.Name = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                //Act
                await _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ValidRequest()
        {
            //Arrange
            var personList = await CreatedPersons();
            if (personList == null) return;
            //Act
            PersonUpdateRequest personUpdateRequest = personList.First().ToPersonUpdateRequest();

            personUpdateRequest.Name = "Phuong";
            personUpdateRequest.Email = "Phuong@gmail.com";

            PersonResponse personUpdateResponse = await
                _personServices.UpdetePerson(personUpdateRequest);
            //print the list
            _outputHelper.WriteLine("Before Update: ");
            foreach (var person in personList)
            {
                _outputHelper.WriteLine(person.ToString());
            }
            //print the updeted list
            var list_after_update = await _personServices.GetAllPersons();
            _outputHelper.WriteLine("After Update: ");
            foreach (var person in list_after_update) 
            {
                _outputHelper.WriteLine(person.ToString());
            }
            var person_updeted = await _personServices.GetPersonByID(personUpdateRequest.PersonID);
            //Assert
            Assert.Equal(person_updeted, personUpdateResponse);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidID()
        {
            //Arrange
            List<PersonResponse> people = await CreatedPersons();
            var person = people.First();

            //Act
            bool isDelete = await _personServices.DeletePerson(person.PersonID);

            //Assert
            Assert.True(isDelete);
        }

        [Fact]
        public async Task DeletePerson_InvalidID()
        {
            //Arrange
            List<PersonResponse> people = await CreatedPersons();
            var person = people.First();

            //Act
            bool isDelete = await _personServices.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDelete);
        }
        #endregion
    }
}
