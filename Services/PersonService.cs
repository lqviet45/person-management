using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services.Helpers;

namespace Services
{
    public class PersonService : IPersonServices
    {
        private readonly PersonsDbContext _db;
        private readonly ICountriesService _countriesService;

        public PersonService(PersonsDbContext personsDbContext, ICountriesService countriesService)
        {
            _db = personsDbContext;
            _countriesService = countriesService;
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

            //_db.Persons.Add(person);

            //_db.SaveChanges();
            _db.Sp_InsertPerson(person);

            return ConvertPersonToPersonResopnse(person);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList()
                .Select(person => ConvertPersonToPersonResopnse(person)).ToList();
            //return _db.Sp_GetAllPersons()
            //    .Select(person => ConvertPersonToPersonResopnse(person)).ToList();
        }

        public PersonResponse? GetPersonByID(Guid? personID)
        {
            if (personID is null) return null;

            var person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);

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
                case nameof(PersonResponse.Name):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Name)) ?
                        temp.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Email)) ?
                        temp.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPerson.Where(temp =>
                        (temp.DateOfBirth != null) ?
                        temp.DateOfBirth.Value.ToString("dd MM yyyy").Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPerson.Where(temp =>
                        (!string.IsNullOrEmpty(temp.Gender)) ?
                        temp.Gender.StartsWith(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;
                case nameof(PersonResponse.CountryID):
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
            //if (personUpdateRequest == null)
            //    throw new ArgumentNullException(nameof(personUpdateRequest));

            ArgumentNullException.ThrowIfNull(personUpdateRequest);

            ValidationHelper.ModelValidation(personUpdateRequest);

            var personUpdate = _db.Persons.FirstOrDefault(temp
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
            
            _db.SaveChanges();

            return ConvertPersonToPersonResopnse(personUpdate);
        }

        public bool DeletePerson(Guid? personID)
        {
            if (personID == null) throw new ArgumentNullException(nameof(personID));

            if (!personID.HasValue) return false;

            var person = _db.Persons.FirstOrDefault(temp => temp.PersonID == personID);
            
            if (person == null) return false;

            _db.Persons.Remove(person);
            _db.SaveChanges();
            return true;
        }
    }
}
