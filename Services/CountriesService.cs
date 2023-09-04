using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesService(ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _countriesRepository.GetCountryByName(countryAddRequest.CountryName) is not null)
            {
                throw new ArgumentException("Country name already exist");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            await _countriesRepository.AddCountry(country);
            return country.ToCountryResponse();
        }

        public async Task<IList<CountryResponse>> GetAllCountries()
        {
            return (await _countriesRepository.GetAllCountries())
                .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByID(Guid? countryID)
        {
            if (countryID is null)
            {
                return null;
            }

            var countryResponse = await _countriesRepository.GetCountryByID(countryID.Value);

            if (countryResponse == null)
            {
                return null;
            }

            return countryResponse.ToCountryResponse();
        }
    }
}