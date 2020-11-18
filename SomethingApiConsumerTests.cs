using System.Collections.Generic;
using System.Threading.Tasks;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace MyPactTest
{
    public class SomethingApiConsumerTests : IClassFixture<ConsumerBogApiPact>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public SomethingApiConsumerTests(ConsumerBogApiPact data)
        {
            _mockProviderService = data.MockProviderService;
            _mockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
            _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
        }

        [Fact]
        public async Task GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
        {
            //Arrange
            _mockProviderService
                .Given("There is a something with id 'tester'")
                .UponReceiving("A GET request to retrieve the something")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/api/content/article/a26ce357-e3c5-4828-2a4b-08d8422b1ed9",
                    //Headers = new Dictionary<string, object>
                    //{
                    //    { "Accept", "application/json" }
                    //}
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new //NOTE: Note the case sensitivity here, the body will be serialised as per the casing defined
                    {
                        Id = "a26ce357-e3c5-4828-2a4b-08d8422b1ed9",
                        Title =  "This works",
                        Author = "mrPt",
                        IsPublished = false
                    }
                }); //NOTE: WillRespondWith call must come last as it will register the interaction

            var consumer = new BogApiContentClient(_mockProviderServiceBaseUri);

            //Act
            var result = await consumer.GetArticleContent("a26ce357-e3c5-4828-2a4b-08d8422b1ed9");

            //Assert
            Assert.Equal("This works", result.Title);

            _mockProviderService.VerifyInteractions(); //NOTE: Verifies that interactions registered on the mock provider are called at least once
        }
    }
}