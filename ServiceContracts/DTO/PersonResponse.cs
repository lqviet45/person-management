using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of 
    /// most methods of Persons service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Country { get; set; }

        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double Age { get; set; }

        /// <summary>
        /// Compares the current object date with the parameter object
        /// </summary>
        /// <param name="obj">The PersonResponse Object to compare</param>
        /// <returns>True or False, indicating whether all
        /// person detail are matched with the specified parameter object</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;
            PersonResponse person = (PersonResponse)obj;
            return PersonID == person.PersonID 
                && Name == person.Name
                && Email == person.Email 
                && DateOfBirth == person.DateOfBirth
                && Gender == person.Gender
                && CountryID == person.CountryID
                && Address == person.Address
                && ReceiveNewsLetters == person.ReceiveNewsLetters
                && Age == person.Age;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Name: {Name}, Email: {Email}, Gender: {Gender}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters,
                CountryID = CountryID,
                Gender = (GenderOptions) Enum.Parse(typeof(GenderOptions), Gender, true)
            };
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person
        /// to PersonResponse
        /// </summary>
        /// <param name="person">The Person type object need to convert</param>
        /// <returns>The converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse() 
            {
                PersonID = person.PersonID,
                Name = person.Name,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null)?
                Math.Round(((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25)) : 0
            };
        }
    }
}
