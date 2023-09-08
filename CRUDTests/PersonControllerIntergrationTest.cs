using System.Net.Http;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace PersonManagementTest
{
    public class PersonControllerIntergrationTest : IClassFixture<CustomWebApplicationFactory>
    {

        private readonly HttpClient _client;

        public PersonControllerIntergrationTest(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage httpResponse = await _client.GetAsync("/persons/Index");

            //Assert
            Assert.True(httpResponse.IsSuccessStatusCode);

            string responseBody = await httpResponse.Content.ReadAsStringAsync();

            HtmlDocument html = new();
            html.LoadHtml(responseBody);

            var document = html.DocumentNode;
            var persons_table = document.QuerySelectorAll("table .persons");

            Assert.NotNull(persons_table);
        }
        #endregion
    }
}
