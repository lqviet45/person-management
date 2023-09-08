using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonService : IPersonServices
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonService> _logger;

        public PersonService(IPersonsRepository personsRepository, ICountriesService countriesService
            , ILogger<PersonService> logger)
        {
            _personsRepository = personsRepository;
            _countriesService = countriesService;
            _logger = logger;
        }

        private PersonResponse ConvertPersonToPersonResopnse(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesService
                .GetCountryByID(person.CountryID).Result?.CountryName;

            return personResponse;
        }

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
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

            await _personsRepository.AddPerson(person);           
            //_db.Sp_InsertPerson(person);

            return ConvertPersonToPersonResopnse(person);
        }

        public async Task<List<PersonResponse>> GetAllPersons()
        {
            var personList = await _personsRepository.GetAllPersons();
            return personList
                .Select(person => ConvertPersonToPersonResopnse(person)).ToList();
            //return _db.Sp_GetAllPersons()
            //    .Select(person => ConvertPersonToPersonResopnse(person)).ToList();
        }

        public async Task<PersonResponse?> GetPersonByID(Guid? personID)
        {
            if (personID is null) return null;

            var person = await _personsRepository.GetPersonByID(personID.Value);

            if (person == null) return null;

            return ConvertPersonToPersonResopnse(person);
        }

        public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString)
        {
            if (string.IsNullOrEmpty(searchString)) searchString = string.Empty;
            List<Person> persons = searchBy switch
            {
                nameof(PersonResponse.Name) =>
                   await _personsRepository.GetFilteredPersons(temp =>
                       temp.Name.Contains(searchString)),

                nameof(PersonResponse.Email) =>
                   await _personsRepository.GetFilteredPersons(temp =>
                       temp.Email.Contains(searchString)),

                nameof(PersonResponse.DateOfBirth) =>
                   await _personsRepository.GetFilteredPersons(temp =>
                       temp.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString)),

                nameof(PersonResponse.Gender) =>
                   await _personsRepository.GetFilteredPersons(temp =>
                       temp.Gender.StartsWith(searchString)),

                _ => await _personsRepository.GetAllPersons()
            };

            return persons.Select(temp => temp.ToPersonResponse()).ToList();
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

        public async Task<PersonResponse> UpdetePerson(PersonUpdateRequest? personUpdateRequest)
        {
            //if (personUpdateRequest == null)
            //    throw new ArgumentNullException(nameof(personUpdateRequest));

            ArgumentNullException.ThrowIfNull(personUpdateRequest);

            ValidationHelper.ModelValidation(personUpdateRequest);

            var personUpdate = await _personsRepository.GetPersonByID(personUpdateRequest.PersonID);

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
            
            await _personsRepository.UpdatePerson(personUpdate);

            return ConvertPersonToPersonResopnse(personUpdate);
        }

        public async Task<bool> DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException(nameof(personID));

            if (!personID.HasValue) return false;

            var person = await _personsRepository.GetPersonByID(personID.Value);
            
            if (person == null) return false;

            await _personsRepository.DeletePerson(personID.Value);

            return true;
        }
    }
}
