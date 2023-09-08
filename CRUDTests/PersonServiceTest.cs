using System;
using System.Collections.Generic;
using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;
using Xunit.Abstractions;
using Moq;
using AutoFixture;
using RepositoryContracts;
using System.Runtime.ConstrainedExecution;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonServices _personServices;
        private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsRepository> _personRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;

        private readonly ITestOutputHelper _outputHelper;
        private readonly IFixture _fixture;

        private readonly ILogger<PersonService> _logger;
        public PersonServiceTest(ITestOutputHelper testOutputHelper, ILogger<PersonService> logger)
        {
            _fixture = new Fixture();
            _logger = logger;

            _personRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personRepositoryMock.Object;

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;

            _countriesService = new CountriesService(_countriesRepository);
            _personServices = new PersonService(_personsRepository, _countriesService, _logger);

            _outputHelper = testOutputHelper;
        }

        public List<Person> CreatedPersons()
        {
            List<Person> personAddRequests = new()
            {
                _fixture.Build<Person>()
                .With(temp => temp.Email, "v@gmail.com")
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "v@gmail.com")
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create(),

                _fixture.Build<Person>()
                .With(temp => temp.Email, "v@gmail.com")
                .With(temp => temp.Gender, GenderOptions.Female.ToString())
                .Create()
            };

            return personAddRequests;
        }


        #region AddPerson
        [Fact]
        public async Task AddPerson_NullPersonRequest_ToBeArgumentNullException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Name, null as string).Create();

            Person person = personAddRequest.ToPerson();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _personServices.AddPerson(personAddRequest));
        }

        [Fact]
        public async Task AddPerson_ValidPerson_ToBeSuccess()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "viet@email.com").Create();

            Person person = personAddRequest.ToPerson();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);

            //Act
            var person_add_response = await _personServices.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_add_response.PersonID;
            //print person_add_response
            _outputHelper.WriteLine($"Expected: {person_response_expected}");
            //print the list are added
            _outputHelper.WriteLine($"Actual: {person_add_response}");
            //Assert
            Assert.True(person_add_response.PersonID != Guid.Empty);
            Assert.Equal(person_add_response, person_response_expected);
        }
        #endregion

        #region GetAllPerson
        [Fact]
        public async Task GetAllPerson_EmptyList()
        {
            //Arrange
            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(new List<Person>());
            //Act
            var person_list = await _personServices.GetAllPersons();

            Assert.Empty(person_list);
        }

        [Fact]
        public async Task GetAllPerson_ValidList_ToBeSuccess()
        {
            //Arrange
            List<Person> persons = CreatedPersons();

            List<PersonResponse> person_response_list_expected = persons
                .Select(person => person.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (var personResponse in person_response_list_expected)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            //print th actual_person_list
            var actual_person_list = await _personServices.GetAllPersons();
            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (var expected_person in person_response_list_expected)
            {
                Assert.Contains(expected_person, actual_person_list);
            }
        }
        #endregion

        #region GetPersonByID
        [Fact]
        public async Task GetPersonByID_NullPersonID_ToBeNull()
        {
            //Arrange
            Guid? personID = null;

            //Acts
            var person_response = await _personServices.GetPersonByID(personID);
            //Assert
            Assert.Null(person_response);
        }

        [Fact]
        public async Task GetPersonByID_ValidPersonID_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "v@email.com").Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            _personRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            //Act
            var person_response = await _personServices.GetPersonByID(person.PersonID);
            
            //Assert
            Assert.Equal(person_response, person_response_expected);
        }
        #endregion

        #region GetFilteredPersons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            List<Person> persons = CreatedPersons();

            List<PersonResponse> person_response_list_expected =
                persons.Select(temp => temp.ToPersonResponse()).ToList();
            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (var personResponse in person_response_list_expected)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);

            //Asct
            //print th actual_person_list
            var actual_person_list_search = await
                _personServices.GetFilteredPersons(nameof(Person.Name),"");

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list_search)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (var expected_person in person_response_list_expected)
            {
                Assert.Contains(expected_person, actual_person_list_search);
            }
        }

        [Fact]
        public async Task GetFilteredPersons_AllowSearchString()
        {
            //Arrange
            List<Person> persons = CreatedPersons()
                .Where(temp => temp.Name != null && temp.Name.Contains('o'))
                .ToList();

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse())
                .ToList();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (var personResponse in person_response_list_expected)
            {
                if (personResponse.Name == null) continue;
                if (personResponse.Name.Contains('o', StringComparison.OrdinalIgnoreCase))
                    _outputHelper.WriteLine(personResponse.ToString());
            }
            _personRepositoryMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                .ReturnsAsync(persons);
            //Act
            //print th actual_person_list
            var actual_person_list_search = await
                _personServices.GetFilteredPersons(nameof(Person.Name), "o");

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in actual_person_list_search)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (var expected_person in person_response_list_expected)
            {
                if (expected_person.Name != null)
                {
                    if (expected_person.Name.Contains('o',
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
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            //Arrange
            List<Person> persons = CreatedPersons();

            List<PersonResponse> person_response_list_expected = persons
                .OrderByDescending(temp => temp.Name, StringComparer.OrdinalIgnoreCase)
                .Select(temp => temp.ToPersonResponse()).ToList();

            _personRepositoryMock.Setup(temp => temp.GetAllPersons())
                .ReturnsAsync(persons);

            var allPersons = await _personServices.GetAllPersons();

            //print the personResponses
            _outputHelper.WriteLine("Expected: ");
            foreach (var personResponse in person_response_list_expected)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //print th actual_person_list
            var person_list_sort =
                _personServices.GetSortedPersons(allPersons, nameof(Person.Name), SortOrderOptions.DESC);

            _outputHelper.WriteLine("Atual: ");
            foreach (PersonResponse personResponse in person_list_sort)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            for (int i = 0; i < person_response_list_expected.Count; i++)
            {
                Assert.Equal(person_response_list_expected[i], person_list_sort[i]);
            }
        }
        #endregion

        #region UpdatePerson
        [Fact]
        public async Task UpdatePerson_NullPersonRequest_ToBeArgumentNullException()
        {
            //Arrange
            PersonUpdateRequest? personUpdateRequest = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                //Act
                await _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
        {
            //Arrange
            PersonUpdateRequest person_update_request = _fixture.Build<PersonUpdateRequest>()
                .With(temp => temp.PersonID, Guid.NewGuid)
                .Create();
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                //Act
                await _personServices.UpdetePerson(person_update_request));
        }

        [Fact]
        public async Task UpdatePerson_NullPersonName_ToBeArgumentException()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Name, null as string)
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            //Arrange
            var personUpdateRequest =  person_response_from_add.ToPersonUpdateRequest();


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                //Act
                await _personServices.UpdetePerson(personUpdateRequest));
        }

        [Fact]
        public async Task UpdatePerson_ValidRequest_ToBeSuccess()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, null as string)
                .With(temp => temp.Email, "Viet@gmail.com")
                .With(temp => temp.Gender, GenderOptions.Male.ToString())
                .Create();
            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest personUpdateRequest = person_response_expected.ToPersonUpdateRequest();

            _personRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            _personRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                .ReturnsAsync(person);
            //Act
            PersonResponse personUpdateResponse = await
                _personServices.UpdetePerson(personUpdateRequest);
            //print the list
            _outputHelper.WriteLine("Before Update: ");

            _outputHelper.WriteLine(person_response_expected.ToString());

            _outputHelper.WriteLine("After Update: ");

            _outputHelper.WriteLine(personUpdateResponse.ToString());

            //Assert
            Assert.Equal(person_response_expected, personUpdateResponse);
        }
        #endregion

        #region DeletePerson
        [Fact]
        public async Task DeletePerson_ValidID_ToBeSuccess()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
               .With(temp => temp.Email, null as string)
               .With(temp => temp.Email, "Viet@gmail.com")
               .With(temp => temp.Gender, GenderOptions.Male.ToString())
               .Create();

            _personRepositoryMock.Setup(temp => temp.DeletePerson(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            _personRepositoryMock.Setup(temp => temp.GetPersonByID(It.IsAny<Guid>()))
                .ReturnsAsync(person);
            //Act
            bool isDelete = await _personServices.DeletePerson(person.PersonID);

            //Assert
            Assert.True(isDelete);
        }

        [Fact]
        public async Task DeletePerson_InvalidID_ToBeFalse()
        {
            //Arrange
            List<Person> people = CreatedPersons();

            //Act
            bool isDelete = await _personServices.DeletePerson(Guid.NewGuid());

            //Assert
            Assert.False(isDelete);
        }
        #endregion
    }
}
