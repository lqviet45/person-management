using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the DTO class that contain person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person ID can't be blank")]
        public Guid PersonID { get; set; }
        [Required(ErrorMessage = "Person name can not be blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email can not be blank")]
        [EmailAddress]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the current PersonUpdateRequest into a new object of 
        /// Person type
        /// </summary>
        /// <returns>A Person</returns>
        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = PersonID,
                Name = Name,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
