using Ardalis.Result;
using DogHouse.Application.Common;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Domain.DTOs;
using DogHouse.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests
{
    [TestClass]
    public class DogControllerTests
    {
        private Mock<IDogService> dogServiceMock;
        private DogsController dogsController;
        private IReadOnlyList<DogDto> sampleDogs;

        [TestInitialize]
        public void Setup()
        {
            dogServiceMock = new Mock<IDogService>();
            dogsController = new DogsController(dogServiceMock.Object);

            var httpContext = new DefaultHttpContext();
            dogsController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            sampleDogs = new List<DogDto>
            {
                new DogDto { Name = "Neo", Colors = "Red,Amber", TailLength = 22.0, Weight = 32.0 },
                new DogDto { Name = "Jessy", Colors = "Black,White", TailLength = 7, Weight = 30.0 },
                new DogDto { Name = "Max", Colors = "Brown", TailLength = 15.0, Weight = 25.0 },
                new DogDto { Name = "Bella", Colors = "Golden", TailLength = 20.0, Weight = 28.0 },
                new DogDto { Name = "Charlie", Colors = "Black", TailLength = 18.0, Weight = 35.0 },
                new DogDto { Name = "Lucy", Colors = "White", TailLength = 10.0, Weight = 22.0 },
                new DogDto { Name = "Cooper", Colors = "Gray", TailLength = 12.0, Weight = 27.0 },
                new DogDto { Name = "Daisy", Colors = "Brown,White", TailLength = 14.0, Weight = 24.0 },
                new DogDto { Name = "Rocky", Colors = "Black,Brown", TailLength = 16.0, Weight = 29.0 },
                new DogDto { Name = "Molly", Colors = "Golden,White", TailLength = 19.0, Weight = 26.0 },
                new DogDto { Name = "Buddy", Colors = "Red", TailLength = 21.0, Weight = 31.0 },
                new DogDto { Name = "Lola", Colors = "Amber", TailLength = 13.0, Weight = 23.0 }
            };
        }

        [TestMethod]
        public void Ping_ReturnsCorrectVersion()
        {
            // Act
            var result = dogsController.Ping() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Dogshouseservice.Version1.0.1", result.Value);
        }

        [TestMethod]
        public async Task GetDogs_ReturnsDogs_WhenSuccessful()
        {
            // Arrange
            var result = Result<IReadOnlyList<DogDto>>.Success(sampleDogs);
            dogServiceMock.Setup(s => s.GetDogsAsync(It.IsAny<DogFitlerDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(result);

            // Act
            dynamic actionResult = await dogsController.GetDogs(null, null, 1, 10);

            // Assert
            Assert.IsNotNull(actionResult);
            var returnedDogs = actionResult.Value as IReadOnlyList<DogDto>;
            Assert.IsNotNull(returnedDogs);
            Assert.AreEqual(sampleDogs.Count, returnedDogs.Count);
        }

        [TestMethod]
        public async Task CreateDog_ReturnsDog_WhenSuccessful()
        {
            // Arrange
            var dogDto = new DogDto { Name = "Buddy" };
            var result = Result<DogDto>.Success(dogDto);
            dogServiceMock.Setup(s => s.CreateDogAsync(It.IsAny<DogDto>())).ReturnsAsync(result);

            // Act
            dynamic actionResult = await dogsController.CreateDog(dogDto);

            // Assert
            Assert.IsNotNull(actionResult);
            var returnedDog = actionResult.Value as DogDto;
            Assert.IsNotNull(returnedDog);
            Assert.AreEqual("Buddy", returnedDog.Name);
        }

        [TestMethod]
        public async Task GetDogs_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var result = Result<IReadOnlyList<DogDto>>.Error("Service error");
            dogServiceMock.Setup(s => s.GetDogsAsync(It.IsAny<DogFitlerDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(result);

            // Act
            dynamic actionResult = await dogsController.GetDogs(null, null, 1, 10);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(422, actionResult.Value.Status);
        }

        [TestMethod]
        public async Task GetDogs_ReturnsFilteredDogs_WhenAttributeAndOrderProvided()
        {
            // Arrange
            var dogDtos = new List<DogDto> { new DogDto { Name = "Buddy" } };
            var result = Result<IReadOnlyList<DogDto>>.Success(dogDtos);
            dogServiceMock.Setup(s => s.GetDogsAsync(It.IsAny<DogFitlerDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(result);

            // Act
            dynamic actionResult = await dogsController.GetDogs("Name", "Asc", 1, 10);

            // Assert
            Assert.IsNotNull(actionResult);
            var returnedDogs = actionResult.Value as IReadOnlyList<DogDto>;
            Assert.IsNotNull(returnedDogs);
            Assert.AreEqual(1, returnedDogs.Count);
            Assert.AreEqual("Buddy", returnedDogs.First().Name);
        }

        [TestMethod]
        public async Task GetDogs_ReturnsEmptyList_WhenNoDogsFound()
        {
            // Arrange
            var result = Result<IReadOnlyList<DogDto>>.Success(new List<DogDto>());
            dogServiceMock.Setup(s => s.GetDogsAsync(It.IsAny<DogFitlerDto>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(result);

            // Act
            dynamic actionResult = await dogsController.GetDogs(null, null, 1, 10);

            // Assert
            Assert.IsNotNull(actionResult);
            var returnedDogs = actionResult.Value as IReadOnlyList<DogDto>;
            Assert.IsNotNull(returnedDogs);
            Assert.AreEqual(0, returnedDogs.Count);
        }
    }
}