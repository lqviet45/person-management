﻿using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating
    /// Person entity
    /// </summary>
    public interface IPersonServices
    {
        /// <summary>
        /// Add a new Person into the list of Person
        /// </summary>
        /// <param name="personAddRequest">Person to add</param>
        /// <returns>The same peron details, along with newly 
        /// generated PersonID</returns>
        /// <exception cref="ArgumentNullException">When PersonAddRequyest is null</exception>
        /// <exception cref="ArgumentException">Name is null</exception>
        PersonResponse AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Get all the Person in the list of person
        /// </summary>
        /// <returns>a list of Personresponse type</returns>
        List<PersonResponse> GetAllPersons();

        /// <summary>
        /// Get a person by there ID
        /// </summary>
        /// <param name="personID">The ID of person</param>
        /// <returns>Matching person object as PersonResopnse type</returns>
        PersonResponse? GetPersonByID(Guid? personID);

        /// <summary>
        /// Returns all person objects that matches with the given 
        /// search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons base on given search field 
        /// and search string</returns>
        List<PersonResponse> GetFilteredPersons(string searchBy, string? searchString);
    }
}
