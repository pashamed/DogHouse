using DogHouse;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class BaseApiTest
    {
        protected HttpClient Client;
        protected WebApplicationFactory<Program> Factory;

        [TestInitialize]
        public void BaseSetup()
        {
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
                    });
                });

            Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5191")
            });
        }

        [TestCleanup]
        public void BaseCleanup()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }
}
