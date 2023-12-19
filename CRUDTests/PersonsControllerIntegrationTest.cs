using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Localization;
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

            string responseBody = await responseMessage.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);

            var document = html.DocumentNode;

            //with this, we can write asserts like: if textbox value exists or not, validation error message displayed or not, submit button exists or not with the help of Fizzler
            document.QuerySelectorAll("table.persons").Should().NotBeNull(); //table tag with css class persons
        }
        #endregion
    }
}
