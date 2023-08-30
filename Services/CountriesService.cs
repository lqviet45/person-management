using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly PersonsDbContext _dbContext;

        public CountriesService(PersonsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (_dbContext.Countries.Count(temp =>
            temp.CountryName == countryAddRequest.CountryName) > 0)
            {
                throw new ArgumentException("Country name already exist");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            _dbContext.Countries.Add(country);

            _dbContext.SaveChanges();

            return country.ToCountryResponse();
        }

        public IList<CountryResponse> GetAllCountries()
        {
            return _dbContext.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByID(Guid? countryID)
        {
            if (countryID is null)
            {
                return null;
            }

            var countryResponse = _dbContext.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (countryResponse == null)
            {
                return null;
            }

            return countryResponse.ToCountryResponse();
        }
    }
}