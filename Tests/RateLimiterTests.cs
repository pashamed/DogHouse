using DogHouse;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Tests
{
    [TestClass]
    public class RateLimiterTests
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;
        private int _permitLimit;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
              .WithWebHostBuilder(builder =>
              {
                  builder.ConfigureAppConfiguration((context, config) =>
                  {
                      config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
                  });
              });

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            _permitLimit = config.GetSection("RateLimitingSettings").GetValue<int>("PermitLimit");
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5191") // Replace with your external API URL
            });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task RateLimiter_AllowsRequestsWithinLimit()
        {
            // Send requests within the allowed limit
            for (int i = 0; i < 10; i++)
            {
                var response = await _client.GetAsync("/ping");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [TestMethod]
        public async Task RateLimiter_ReturnsTooManyRequests_WhenLimitExceeded()
        {
            int totalRequests = _permitLimit + 5;
            var tasks = Enumerable.Range(0, totalRequests).Select(_ => _client.GetAsync("/ping"));
            var responses = await Task.WhenAll(tasks);

            var okReponsesCount = responses.Count(r => r.StatusCode == HttpStatusCode.OK);
            var tooManyRequestsCount = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);

            Assert.AreEqual(_permitLimit, okReponsesCount, $"Expected {_permitLimit} requests to return 200 OK.");
            Assert.AreEqual(5, tooManyRequestsCount, "Expected 5 requests to return 429 Too Many Requests.");
        }
    }
}