using ServiceContracts.DTO;
using ServiceContracts.Enums;

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
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);

        /// <summary>
        /// Get all the Person in the list of person
        /// </summary>
        /// <returns>a list of Personresponse type</returns>
        Task<List<PersonResponse>> GetAllPersons();

        /// <summary>
        /// Get a person by there ID
        /// </summary>
        /// <param name="personID">The ID of person</param>
        /// <returns>Matching person object as PersonResopnse type</returns>
        Task<PersonResponse?> GetPersonByID(Guid? personID);

        /// <summary>
        /// Returns all person objects that matches with the given 
        /// search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons base on given search field 
        /// and search string</returns>
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list if persons to sort</param>
        /// <param name="sortBy">Name of the propety (key), base
        /// on which the list should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted list of person as PersonResponse list</returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy,
            SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update</param>
        /// <returns>Returns the person response object updated</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        Task<PersonResponse> UpdetePerson(PersonUpdateRequest? personUpdateRequest);

        /// <summary>
        /// Delete a specified person detals based on the given person ID
        /// </summary>
        /// <param name="personID">Person ID to delete</param>
        /// <returns>True, if the deleteions is successful;
        /// otherwise return false</returns>
        Task<bool> DeletePerson(Guid? personID);
    }
}
