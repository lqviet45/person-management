using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Xunit;
using EntityFrameworkCoreMock;
using Moq;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            var countriesInitialData = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                    new DbContextOptionsBuilder<ApplicationDbContext>().Options
                );

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp 
                => temp.Countries, countriesInitialData);

            _countriesService = new CountriesService(dbContext);
        }

        #region AddCountry
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;
            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                //Act
                await _countriesService.AddCountry(request));      
        }

        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = null
            };
            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                //Act
                await _countriesService.AddCountry(request));
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
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
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await _countriesService.AddCountry(request1);
                await _countriesService.AddCountry(request2);
            });
        }

        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest()
            {
                CountryName = "Japan"
            };
            //ACT
            CountryResponse response = await _countriesService.AddCountry(request);
            var countries_from_GetAllCountries =
                await _countriesService.GetAllCountries();
            //Assert
            Assert.True(response.CountryID != Guid.Empty);
            Assert.Contains(response, countries_from_GetAllCountries);
        }
        #endregion

        #region GetAllCountries
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            IList<CountryResponse> actual_countries = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_countries);
        }

        [Fact]
        public async Task GetAllCountries_AddFewCounties()
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
                countryResponses.Add(await _countriesService.AddCountry(country));
            }

            IList<CountryResponse> actualCountryResponseList = await
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
        public async Task GetCountryByID_NullID()
        {
            Guid?  countryID = null;

            CountryResponse? countryResponse = await _countriesService.GetCountryByID(countryID);
            
            Assert.Null(countryResponse);
        }

        [Fact]
        public async Task GetCountryByID_ValidID()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest()
            {
                CountryName = "China"
            };
            var countryResponse = await
                _countriesService.AddCountry(countryAddRequest);
            //Act
            var actual_country_from_get = await _countriesService
                .GetCountryByID(countryResponse.CountryID);

            //Assert
            Assert.Equal(countryResponse, actual_country_from_get);
        }
        #endregion
    }
}