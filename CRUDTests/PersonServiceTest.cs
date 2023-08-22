using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using Xunit;

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonServices _personServices;

        public PersonServiceTest() 
        {
            _personServices = new PersonService();
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

            //Assert
            Assert.True(person_add_response.PersonID != Guid.Empty);
            Assert.Contains(person_add_response, list_actual_person);
        }
        #endregion
    }
}
