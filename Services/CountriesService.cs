using Entities;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        private readonly IList<Country> _countries;

        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();
            if (initialize)
            {
                //Guid = 2AA96B5F-DF73-4268-83A1-08ABD5CA3C92
                //Guid = 417BA260-862D-4388-B2A8-E7B4B3F4A076
                //Guid = E5910069-8A9A-4BAE-869F-DEFDA863451A
                _countries.Add(new Country()
                {
                    CountryID = Guid.Parse("2AA96B5F-DF73-4268-83A1-08ABD5CA3C92"),
                    CountryName = "Viet Nam"
                });
                _countries.Add(new Country()
                {
                    CountryID = Guid.Parse("417BA260-862D-4388-B2A8-E7B4B3F4A076"),
                    CountryName = "Japan"
                });
                _countries.Add(new Country()
                {
                    CountryID = Guid.Parse("E5910069-8A9A-4BAE-869F-DEFDA863451A"),
                    CountryName = "China"
                });
            }
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