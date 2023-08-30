using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService(new 
                PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
            //Assert
            Assert.Throws<ArgumentNullException>(() =>
                //Act
                _countriesService.AddCountry(request));      
        }

        [Fact]
        public void AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
                //Act
                _countriesService.AddCountry(request));
        }

        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest()
            {
                CountryName = "USA"
            };

            CountryAddRequest? request2 = new CountryAddRequest()
            {
                CountryName = "USA"
            };
            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _countriesService.AddCountry(request1);
                _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public void AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            //ACT
            CountryResponse response = _countriesService.AddCountry(request);
            var countries_from_GetAllCountries =
                _countriesService.GetAllCountries();
            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Acts
            IList<CountryResponse> actual_countries = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_countries);
        }

        [Fact]
        public void GetAllCountries_AddFewCounties()
        {
            IList<CountryAddRequest> country_request_list = new
                List<CountryAddRequest>()
            {
                new CountryAddRequest() {CountryName = "VN"},
                new CountryAddRequest() {CountryName = "USA"},
                new CountryAddRequest() {CountryName = "JP"}
            };

            //Act
            IList<CountryResponse> countryResponses
                = new List<CountryResponse>();
            foreach (var country in country_request_list) 
            {
                countryResponses.Add(_countriesService.AddCountry(country));
            }

            IList<CountryResponse> actualCountryResponseList =
                _countriesService.GetAllCountries();

            //Assert
            foreach (var expected_country in countryResponses)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }
        #endregion

        #region GetCountryByID
        [Fact]
        public void GetCountryByID_NullID()
        {
            Guid?  countryID = null;

            CountryResponse? countryResponse = _countriesService.GetCountryByID(countryID);
            
            Assert.Null(countryResponse);
        }

        [Fact]
        public void GetCountryByID_ValidID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            var countryResponse =
                _countriesService.AddCountry(countryAddRequest);
            //Act
            var actual_country_from_get = _countriesService
                .GetCountryByID(countryResponse.CountryID);

            //Assert
            Assert.Equal(countryResponse, actual_country_from_get);
        }
        #endregion
    }
}