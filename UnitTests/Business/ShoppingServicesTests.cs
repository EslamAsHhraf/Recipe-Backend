using Business_Access_Layer.Concrete;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using FakeItEasy;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UnitTests.Business
{
    public class ShoppingServicesTests
    {
        private ShoppingServices _shoppingServices;
        private readonly IAuthService _userService;
        private readonly IShopping _shopping;
        private readonly IRepository<Shopping> _shoppingRepository;

        public ShoppingServicesTests()
        {
            _userService = A.Fake<IAuthService>();
            _shopping = A.Fake<IShopping>();
            _shoppingRepository = A.Fake<IRepository<Shopping>>();
            _shoppingServices = new ShoppingServices(_userService, _shopping, _shoppingRepository);
        }

        [Fact]
        public async Task GetById_ReturnsShopping()
        {
            // Arrange
            int shoppingId = 1;
            var expectedShopping = new Shopping { Id = shoppingId, Title = "ShoppingItem" };
            A.CallTo(() => _shoppingRepository.GetById(shoppingId)).Returns(expectedShopping);

            // Act
            var response = await _shoppingServices.GetById(shoppingId);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedShopping, response.Data);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound()
        {
            // Arrange
            var expectedData = new { Title = "No Content" };

            int shoppingId = 1;
            A.CallTo(() => _shoppingRepository.GetById(shoppingId)).Returns((Shopping)null); // Simulate shopping not found

            // Act
            var response = await _shoppingServices.GetById(shoppingId);

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }

        [Fact]
        public async Task GetMyShopping_ReturnsShoppingList()
        {
            // Arrange
            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns(userData);
            var expectedShoppingList = new List<Shopping>
            {
                new Shopping { Id = 1, Title = "Shopping1" },
                new Shopping { Id = 2, Title = "Shopping2" }
            };
            int shoppingId = 1;
            var expectedShopping = new Shopping { Id = shoppingId, Title = "ShoppingItem" };
            A.CallTo(() => _shoppingRepository.GetById(shoppingId)).Returns(expectedShopping);

            A.CallTo(() => _shopping.GetShopping(userData.Id)).Returns(expectedShoppingList);

            // Act
            var response = await _shoppingServices.GetMyShopping();

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedShoppingList, response.Data);
        }

        [Fact]
        public async Task GetMyShopping_ReturnsUnauthorized()
        {
            var expectedData = new { Title = "Unauthorized" };

            // Arrange
            A.CallTo(() => _userService.GetMe()).Returns((UserData)null); // Simulate unauthorized user

            // Act
            var response = await _shoppingServices.GetMyShopping();

            // Assert
            Assert.Equal("401", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task GetMyPurchased_ReturnsPurchasedList()
        {
            // Arrange
            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns(userData);
            var expectedPurchasedList = new List<Shopping>
            {
                new Shopping { Id = 1, Title = "Purchased1" },
                new Shopping { Id = 2, Title = "Purchased2" }
            };
            A.CallTo(() => _shopping.GetPurchased(userData.Id)).Returns(expectedPurchasedList);

            // Act
            var response = await _shoppingServices.GetMyPurchased();

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedPurchasedList, response.Data);
        }

        [Fact]
        public async Task GetMyPurchased_ReturnsUnauthorized()
        {
            // Arrange
            var expectedData = new { Title = "Unauthorized" };

            A.CallTo(() => _userService.GetMe()).Returns((UserData)null); // Simulate unauthorized user

            // Act
            var response = await _shoppingServices.GetMyPurchased();

            // Assert
            Assert.Equal("401", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }

        [Fact]
        public async Task AddPurchased_ReturnsUpdatedShopping()
        {
            // Arrange
            int shoppingId = 1;
            int quantity = 5;
            var shopping = new Shopping { Id = shoppingId, QuantityShopping = 10, QuantityPurchased = 0, CreatedBy=1 };
            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns(userData);
            A.CallTo(() => _shoppingRepository.GetById(shoppingId)).Returns(shopping);
            A.CallTo(() => _shoppingRepository.Update(shopping)).Returns(shopping);
            // Act
            var response = await _shoppingServices.AddPurchased(shoppingId, quantity);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(shopping, response.Data);
        }

        [Theory]
        [InlineData(1)] 
        [InlineData(2)]    

        public async Task AddPurchased_ReturnsUnauthorizedUser(int userId)
        {
            // Arrange
            var expectedData = new { Title = "Untheorized user" };

            int shoppingId = 1;
            int quantity = 5;
            var userData = new UserData { Id = 1, Name = "User1" };
            var shopping = new Shopping { Id = shoppingId, QuantityShopping = 10, QuantityPurchased = 0, CreatedBy = userId };

            A.CallTo(() => _userService.GetMe()).Returns((UserData)null); // Simulate unauthorized user
            A.CallTo(() => _shoppingRepository.GetById(shoppingId)).Returns(shopping);

            // Act
            var response = await _shoppingServices.AddPurchased(shoppingId, quantity);

            // Assert
            Assert.Equal("401", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task AddShopping_ReturnsShoppingList()
        {
            // Arrange
            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns(userData);
            var shoppingList = new List<Shopping>
            {
                new Shopping { Title = "Item1", CreatedBy = userData.Id },
                new Shopping { Title = "Item2", CreatedBy = userData.Id }
            };

            // Simulate that checking for existing items returns null, indicating no existing items
            A.CallTo(() => _shopping.check(A<string>._, userData.Id)).Returns((Shopping)null);

            // Simulate successful creation for all shopping items
            A.CallTo(() => _shoppingRepository.Create(A<Shopping>._)).ReturnsNextFromSequence(shoppingList.ToArray());

            // Act
            var response = await _shoppingServices.AddShopping(shoppingList);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(shoppingList, response.Data);
        }

        [Fact]
        public async Task AddShopping_ReturnsUnauthorizedUser()
        {
            // Arrange
            var expectedData = new { Title = "Unauthorize user" };

            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns((UserData)null); // Simulate unauthorized user
            var shoppingList = new List<Shopping>
            {
                new Shopping { Title = "Item1", CreatedBy = userData.Id },
                new Shopping { Title = "Item2", CreatedBy = userData.Id }
            };

            // Act
            var response = await _shoppingServices.AddShopping(shoppingList);

            // Assert
            Assert.Equal("401", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }

        [Fact]
        public async Task AddShopping_ReturnsExistingItemsUpdated()
        {
            // Arrange
            var userData = new UserData { Id = 1, Name = "User1" };
            A.CallTo(() => _userService.GetMe()).Returns(userData);
            var shoppingList = new List<Shopping>
            {
                new Shopping { Title = "Item1", CreatedBy = userData.Id },
                new Shopping { Title = "Item2", CreatedBy = userData.Id }
            };

            // Simulate that checking for existing items returns an existing item
            var existingItem = new Shopping { Title = "Item1", CreatedBy = userData.Id };
            A.CallTo(() => _shopping.check(existingItem.Title, existingItem.CreatedBy)).Returns(existingItem);

            // Simulate successful update for the existing item
            A.CallTo(() => _shoppingRepository.Update(A<Shopping>._)).Returns(existingItem);

            // Act
            var response = await _shoppingServices.AddShopping(shoppingList);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(shoppingList, response.Data);
        }

    }
}