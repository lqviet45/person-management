using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

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

            if (string.IsNullOrEmpty(personAddRequest.Name))
            {
                throw new ArgumentException("Person name is blank!!");
            }

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            _Persons.Add(person);

            return ConvertPersonToPersonResopnse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _Persons.Select(person => person.ToPersonResponse()).ToList();
        }
    }
}
