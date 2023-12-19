using FluentAssertions;
using System.Net.Http;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest: IClassFixture<CustomWebApplicationFactory> //IClassFixture<> provides the object of CustomWebApplicationFactory
    {
        private readonly HttpClient _httpClient;

        public PersonsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage responseMessage = await _httpClient.GetAsync("/Persons/Index");

            //Assert
            responseMessage.Should().BeSuccessful(); //2xx status code
        }

        #endregion
    }
}
