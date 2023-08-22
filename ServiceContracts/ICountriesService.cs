using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represents business logic for manipulating
    /// country entity
    /// </summary>
    public interface ICountriesService
    {
        /// <summary>
        /// Adds a country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">Country object to add</param>
        /// <returns>Return the country object after adding it 
        /// (including newly generated country id)</returns>
        /// <exception cref="ArgumentNullException">Null Request</exception>
        /// <exception cref="ArgumentException">Invalid Name or ID</exception>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);

        /// <summary>
        /// Get all the countries in the list of countries
        /// </summary>
        /// <returns>A list of countryResponses from the list of counties</returns>
        IList<CountryResponse> GetAllCountries();

        /// <summary>
        /// Get a country by contry ID
        /// </summary>
        /// <param name="countryID">The ID of the country</param>
        /// <returns>Matching country as CountryResponse</returns>
        CountryResponse? GetCountryByID(Guid? countryID);
    }
}