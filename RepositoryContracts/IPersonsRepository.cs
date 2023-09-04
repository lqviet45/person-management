using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Person entity
    /// </summary>
    public interface IPersonsRepository
    {
        /// <summary>
        /// Adds a person object to the data store
        /// </summary>
        /// <param name="person">Person object to add</param>
        /// <returns>Returns the person object after adding it to the table</returns>
        Task<Person> AddPerson(Person person);

        /// <summary>
        /// Returns all person in the data store
        /// </summary>
        /// <returns>List of person object from table</returns>
        Task<List<Person>> GetAllPersons();

        /// <summary>
        /// Returns a person object based on the given person ID
        /// </summary>
        /// <param name="personID">the person ID to search</param>
        /// <returns>Matching person or null</returns>
        Task<Person?> GetPersonByID(Guid personID);

        /// <summary>
        /// Returns all person objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching persons with given condition</returns>
        Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate);

        /// <summary>
        /// Daletes a person object base on the person ID
        /// </summary>
        /// <param name="personID">person ID to delete</param>
        /// <returns>returns true if success, otherwise returns false</returns>
        Task<bool> DeletePerson(Guid personID);

        /// <summary>
        /// Update person object based on the given person id
        /// </summary>
        /// <param name="person">person object to update</param>
        /// <returns>Returns the updated person object</returns>
        Task<Person> UpdatePerson(Person person);


    }
}
