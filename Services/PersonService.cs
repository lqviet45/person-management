using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
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

        public List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPerson = GetAllPersons();
            List<PersonResponse> matchingPersons = allPerson;

            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(searchString))
            {
                return matchingPersons;
            }

            switch (searchBy)
            {
                case nameof(Person.Name):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Name)) ?
                        temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.Email):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Email)) ?
                        temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.DateOfBirth):
                    matchingPersons = allPerson.Where(temp =>
                        (temp.DateOfBirth != null) ?
                        temp.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(Person.Gender):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Gender)) ?
                        temp.Gender.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(Person.CountryID):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Country)) ?
                        temp.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                default: matchingPersons = allPerson; break;
            }
            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (string.IsNullOrEmpty(sortBy))
                return allPersons;

            List<PersonResponse> sortedPersons = (sortBy, sortOrder)
            switch
            {
                (nameof(PersonResponse.Name), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Name,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Name), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Name,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC)
                => allPersons.OrderBy(temp => temp.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.DESC)
                => allPersons.OrderByDescending(temp => temp.Email,
                StringComparer.OrdinalIgnoreCase).ToList(),

                _ => allPersons //default case
            };

            return sortedPersons;
        }

        public PersonResponse UpdetePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            ValidationHelper.ModelValidation(personUpdateRequest);

            var personUpdate = _Persons.FirstOrDefault(temp
                => temp.PersonID == personUpdateRequest.PersonID);

            if (personUpdate == null)
                throw new ArgumentException("Given person ID doesn't exist");

            //update all details
            personUpdate.Name = personUpdateRequest.Name;
            personUpdate.Email = personUpdateRequest.Email;
            personUpdate.Address = personUpdateRequest.Address;
            personUpdate.Gender = personUpdateRequest.Gender.ToString();
            personUpdate.ReceiveNewsLetters = personUpdateRequest.ReceiveNewsLetters;
            personUpdate.CountryID = personUpdateRequest.CountryID;
            personUpdate.DateOfBirth = personUpdateRequest.DateOfBirth;
            
            return personUpdate.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException(nameof(personID));

            if (!personID.HasValue) return false;

            var person = _Persons.FirstOrDefault(temp => temp.PersonID == personID);
            
            if (person == null) return false;

            return _Persons.Remove(person);

        }
    }
}
