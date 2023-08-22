using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly IList<Country> _countries;

        public CountriesService()
        {
            _countries = new List<Country>();
        }

        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if(countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (_countries.Where(temp => 
            temp.CountryName == countryAddRequest.CountryName).Count() > 0)
            {
                throw new ArgumentException("Country name already exist");
            }

            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            _countries.Add(country);

            return country.ToCountryResponse();
        }

        public IList<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByID(Guid? countryID)
        {
            if (countryID is null)
            {
                return null;
            }

            var countryResponse = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (countryResponse == null)
            {
                return null;
            }

            return countryResponse.ToCountryResponse();
        }
    }
}