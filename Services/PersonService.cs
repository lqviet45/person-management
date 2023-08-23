using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Services.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class PersonService : IPersonServices
    {
        private readonly List<Person> _Persons;
        private readonly ICountriesService _countriesService;

        public PersonService()
        {
            _Persons = new List<Person>();
            _countriesService = new CountriesService();
        }

        private PersonResponse ConvertPersonToPersonResopnse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService
                .GetCountryByID(person.CountryID)?.CountryName;

            return personResponse;
        }

        public PersonResponse AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest is null)
            {
                throw new ArgumentNullException(nameof(personAddRequest));
            }

            //Model validations
            ValidationHelper.ModelValidation(personAddRequest);

            //Create person
            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            _Persons.Add(person);

            return ConvertPersonToPersonResopnse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _Persons.Select(person => person.ToPersonResponse()).ToList();
        }

        public PersonResponse? GetPersonByID(Guid? personID)
        {
            if (personID is null) return null;

            var person = _Persons.FirstOrDefault(temp => temp.PersonID == personID);

            if (person == null) return null;

            return ConvertPersonToPersonResopnse(person);
        }
    }
}
