using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly ApplicationDbContext _dbContext;

        public CountriesService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

            if (await _dbContext.Countries.CountAsync(temp =>
            temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Country name already exist");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            await _dbContext.Countries.AddAsync(country);

            await _dbContext.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<IList<CountryResponse>> GetAllCountries()
        {
            return await _dbContext.Countries
                .Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByID(Guid? countryID)
        {
            if (countryID is null)
            {
                return null;
            }

            var countryResponse = await _dbContext.Countries
                .FirstOrDefaultAsync(temp => temp.CountryID == countryID);

            if (countryResponse == null)
            {
                return null;
            }

            return countryResponse.ToCountryResponse();
        }
    }
}