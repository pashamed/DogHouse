using Ardalis.Result;
using DogHouse.Application.Common.Interfaces;
using DogHouse.Application.Common;
using DogHouse.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [TestInitialize]
        public void Setup()
        {
            dogRepositoryMock = new Mock<IDogRepository>();
            mapperMock = new Mock<IMapper>();
            validatorMock = new Mock<IValidator<DogDto>>();
            dogService = new DogService(dogRepositoryMock.Object, mapperMock.Object, validatorMock.Object);
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
        public async Task GetDogsAsync_ReturnsDogDtos_WhenSuccessful()
        {
            // Arrange
            var filter = new DogFitlerDto();
            var pageNumber = 1;
            var pageSize = 10;
            var dogs = new List<Dog> { new Dog { Id = 1, Name = "Buddy" } };
            var dogDtos = new List<DogDto> { new DogDto { Name = "Buddy" } };
            dogRepositoryMock.Setup(r => r.GetAllDogsAsync(filter, pageNumber, pageSize)).ReturnsAsync(dogs);
            mapperMock.Setup(m => m.Map<List<DogDto>>(dogs)).Returns(dogDtos);

            // Act
            var result = await dogService.GetDogsAsync(filter, pageNumber, pageSize);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, result.Value.Count);
            Assert.AreEqual("Buddy", result.Value.First().Name);
        }
    }
}
