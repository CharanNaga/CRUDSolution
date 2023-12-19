using FluentAssertions;
using System.Net.Http;

namespace CRUDTests
{
    public class PersonsControllerIntegrationTest
    {
        #region Index
        [Fact]
        public void Index_ToReturnView()
        {
            //Arrange

            //Act
            HttpResponseMessage responseMessage = _client.GetAsync("/Persons/Index");

            //Assert
            responseMessage.Should().BeSuccessful();
        }

        #endregion
    }
}
