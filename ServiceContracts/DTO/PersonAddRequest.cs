﻿using Entities;
using ServiceContracts.Enums;
namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a preson
    /// </summary>
    public class PersonAddRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the current PersonAddRequest into a new object of 
        /// Person type
        /// </summary>
        /// <returns>A Person</returns>
        public Person ToPerson()
        {
            return new Person()
            {
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
