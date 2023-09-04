using Entities;

namespace RepositoryContracts
{
    /// <summary>
    /// Represents date access logic for managing Country entity
    /// </summary>
    public interface ICountriesRepository
    {
        /// <summary>
        /// Adds a new country object to data store
        /// </summary>
        /// <param name="country">Country object to add</param>
        /// <returns>Returns the country object after adding it to the datastore</returns>
        Task<Country> AddCountry(Country country);

        /// <summary>
        /// Returns all countries in th data store
        /// </summary>
        /// <returns>All countries from the table</returns>
        Task<List<Country>> GetAllCountries();

        /// <summary>
        /// Returns a country object base on  the given country id
        /// </summary>
        /// <param name="countryID">the country ID to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByID(Guid countryID);

        /// <summary>
        /// Returns a country object base on trh given country name
        /// </summary>
        /// <param name="name">Country name to search</param>
        /// <returns>Matching country or null</returns>
        Task<Country?> GetCountryByName(string name);

    }
}