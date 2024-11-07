using DogHouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Tests
{
    [TestClass]
    public class RateLimiterTests : BaseApiTest
    {
        private int _permitLimit;

        [TestInitialize]
        public void Setup()
        {
            BaseSetup();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            _permitLimit = config.GetSection("RateLimitingSettings").GetValue<int>("PermitLimit");
        }

        [TestMethod]
        public async Task RateLimiter_AllowsRequestsWithinLimit()
        {
            // Send requests within the allowed limit
            for (int i = 0; i < 10; i++)
            {
                var response = await Client.GetAsync("/ping");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RateLimiter_ReturnsTooManyRequests_WhenLimitExceeded()
        {
            int totalRequests = _permitLimit + 5;
            var tasks = Enumerable.Range(0, totalRequests).Select(_ => Client.GetAsync("/ping"));
            var responses = await Task.WhenAll(tasks);

            var okReponsesCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
            var tooManyRequestsCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

            Assert.AreEqual(_permitLimit, okReponsesCount, $"Expected {_permitLimit} requests to return 200 OK.");
            Assert.AreEqual(5, tooManyRequestsCount, "Expected 5 requests to return 429 Too Many Requests.");
        }
    }
}