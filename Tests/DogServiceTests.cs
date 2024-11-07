using DogHouse.Application.Common;
using DogHouse.Domain.DTOs;
using DogHouse.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using FluentValidation;
using DogHouse.Application.Services;
using FluentValidation.Results;
using DogHouse.Domain.Entities;

namespace Tests
{
    [TestClass]
    public class DogServiceTests
    {
        private Mock<IDogRepository> dogRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<IValidator<DogDto>> validatorMock;
        private DogService dogService;
        private List<Dog> sampleDogs;

        [TestInitialize]
        public void Setup()
        {
            dogRepositoryMock = new Mock<IDogRepository>();
            mapperMock = new Mock<IMapper>();
            validatorMock = new Mock<IValidator<DogDto>>();
            dogService = new DogService(dogRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

            sampleDogs = new List<Dog>
            {
                new Dog { Id = 1, Name = "Neo", Colors = new List<string> { "Red", "Amber" }, TailLength = 22.0, Weight = 32.0 },
                new Dog { Id = 2, Name = "Jessy", Colors = new List<string> { "Black", "White" }, TailLength = 7, Weight = 30.0 },
                new Dog { Id = 3, Name = "Max", Colors = new List<string> { "Brown" }, TailLength = 15.0, Weight = 25.0 }
            };
        }

        [TestMethod]
        public async Task GetDogsAsync_ReturnsFilteredDogs_WhenFilterIsApplied()
        {
            // Arrange
            var filter = new DogFitlerDto
            {
                Attributes = new List<string> { "name" },
                Orders = new List<string> { "asc" }
            };
            dogRepositoryMock.Setup(repo => repo.GetAllDogsAsync(filter, 1, 10)).ReturnsAsync(sampleDogs);
            mapperMock.Setup(m => m.Map<List<DogDto>>(It.IsAny<List<Dog>>())).Returns(new List<DogDto>
            {
                new DogDto { Name = "Jessy", Colors = "Black,White", TailLength = 7, Weight = 30.0 },
                new DogDto { Name = "Max", Colors = "Brown", TailLength = 15.0, Weight = 25.0 },
                new DogDto { Name = "Neo", Colors = "Red,Amber", TailLength = 22.0, Weight = 32.0 }
            });

            // Act
            var result = await dogService.GetDogsAsync(filter, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            var returnedDogs = result.Value;
            Assert.AreEqual(3, returnedDogs.Count);
            Assert.AreEqual("Jessy", returnedDogs[0].Name);
            Assert.AreEqual("Max", returnedDogs[1].Name);
            Assert.AreEqual("Neo", returnedDogs[2].Name);
        }

        [TestMethod]
        public async Task CreateDogAsync_ReturnsInvalid_WhenValidationFails()
        {
            // Arrange
            var dogDto = new DogDto { Name = "Buddy" };
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure("Name", "Name is required") });
            validatorMock.Setup(v => v.ValidateAsync(dogDto, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

            // Act
            var result = await dogService.CreateDogAsync(dogDto);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Name is required", result.ValidationErrors.First().ErrorMessage);
        }

        [TestMethod]
        public async Task CreateDogAsync_ReturnsError_WhenDogNameExists()
        {
            // Arrange
            var dogDto = new DogDto { Name = "Buddy" };
            validatorMock.Setup(v => v.ValidateAsync(dogDto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            dogRepositoryMock.Setup(r => r.DogExistsAsync(dogDto.Name)).ReturnsAsync(true);

            // Act
            var result = await dogService.CreateDogAsync(dogDto);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Dog with the same name already exists.", result.Errors.First());
        }

        [TestMethod]
        public async Task CreateDogAsync_ReturnsDogDto_WhenSuccessful()
        {
            // Arrange
            var dogDto = new DogDto { Name = "Buddy" };
            var dog = new Dog { Name = "Buddy" };
            var newDog = new Dog { Id = 1, Name = "Buddy" };
            validatorMock.Setup(v => v.ValidateAsync(dogDto, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
            dogRepositoryMock.Setup(r => r.DogExistsAsync(dogDto.Name)).ReturnsAsync(false);
            mapperMock.Setup(m => m.Map<Dog>(dogDto)).Returns(dog);
            dogRepositoryMock.Setup(r => r.AddDogAsync(dog)).ReturnsAsync(newDog);
            mapperMock.Setup(m => m.Map<DogDto>(newDog)).Returns(new DogDto { Name = "Buddy" });

            // Act
            var result = await dogService.CreateDogAsync(dogDto);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Buddy", result.Value.Name);
        }

        [TestMethod]
        public async Task GetDogsAsync_ReturnsError_WhenInvalidPageNumberOrPageSize()
        {
            // Arrange
            var filter = new DogFitlerDto();
            var invalidPageNumber = 0;
            var invalidPageSize = 0;

            // Act
            var result = await dogService.GetDogsAsync(filter, invalidPageNumber, invalidPageSize);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Page number and page size must be greater than 0", result.Errors.First());
        }

        [TestMethod]
        public async Task GetDogsAsync_ReturnsError_WhenRepositoryThrowsException()
        {
            // Arrange
            var filter = new DogFitlerDto();
            var pageNumber = 1;
            var pageSize = 10;
            dogRepositoryMock.Setup(r => r.GetAllDogsAsync(filter, pageNumber, pageSize)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await dogService.GetDogsAsync(filter, pageNumber, pageSize);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("An error occurred while getting dogs: Database error", result.Errors.First());
        }

        [TestMethod]
        public async Task GetDogsAsync_ReturnsEmptyList_WhenNoDogsFound()
        {
            // Arrange
            var filter = new DogFitlerDto();
            var pageNumber = 1;
            var pageSize = 10;
            var dogs = new List<Dog>();
            dogRepositoryMock.Setup(r => r.GetAllDogsAsync(filter, pageNumber, pageSize)).ReturnsAsync(dogs);
            mapperMock.Setup(m => m.Map<List<DogDto>>(dogs)).Returns(new List<DogDto>());

            // Act
            var result = await dogService.GetDogsAsync(filter, pageNumber, pageSize);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.Value.Count);
        }
    }
}