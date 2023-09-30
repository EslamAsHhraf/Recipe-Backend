using Business_Access_Layer.Abstract;
using Business_Access_Layer.Concrete;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Business
{
    public class RecipesServicesTests
    {
        private RecipesServices _recipesServices;
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IAuthService _userService;
        private readonly IFileServices _fileServices;
        private readonly IRecipes _recipesRepository;
        public RecipesServicesTests()
        {
            _recipeRepository = A.Fake<IRepository<Recipe>>();
            _userService = A.Fake<IAuthService>();
            _fileServices = A.Fake<IFileServices>();
            _recipesRepository = A.Fake<IRecipes>();
            _recipesServices = new RecipesServices(_recipeRepository, _userService, _fileServices, _recipesRepository);
        }
        [Fact]
        public async Task GetRecipes_ReturnsAllRecipes()
        {
            // Arrange
            var expectedRecipes = A.Fake<List<Recipe>>();
            A.CallTo(() => _recipeRepository.GetAll()).Returns(expectedRecipes);

            // Act
            var response = await _recipesServices.GetAllRecipes();

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedRecipes, response.Data);
        }
        [Fact]
        public async Task GetRecipes_ReturnsNoData()
        {
            // Arrange
            var expectedData = new { Title = "No Content" };


            A.CallTo(() => _recipeRepository.GetAll()).Returns(null); 

            // Act
            var response = await _recipesServices.GetAllRecipes();

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);

        }
        [Fact]
        public async Task GetRecipes_ReturnsRecipe_ById()
        {
            // Arrange
            var expectedRecipe = A.Fake<Recipe>();
            A.CallTo(() => _recipeRepository.GetById(1)).Returns(expectedRecipe);

            // Act
            var response = await _recipesServices.GetRecipeById(1);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedRecipe, response.Data);
        }
        [Fact]
        public async Task GetRecipes_ReturnsRecipe_ById_NotFound()
        {
            // Arrange
            var expectedRecipe = new { Title = "No Content" };
            A.CallTo(() => _recipeRepository.GetById(1)).Returns((Recipe) null);

            // Act
            var response = await _recipesServices.GetRecipeById(1);

            // Assert
            Assert.Equal("404", response.Status);
            expectedRecipe.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task GetMyRecipes_ReturnsRecipesOfUser_ByUSerId_auth()
        {
            // Arrange
            var userModel = A.Fake<UserData>();
            var expectedRecipes = A.Fake<List<Recipe>>();
            A.CallTo(() => _userService.GetMe()).Returns(userModel);

            A.CallTo(() => _recipesRepository.GetMyRecipes(userModel.Id)).Returns(expectedRecipes);

            // Act
            var response = await _recipesServices.GetMyRecipes();

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedRecipes, response.Data);
        }
        [Fact]
        public async Task GetMyRecipes_ReturnsRecipesOfUser_ByUSerId_Unauth()
        {
            // Arrange
            var userModel = A.Fake<UserData>();
            var expectedRes = new { Title = "Unauthorized" };
            A.CallTo(() => _userService.GetMe()).Returns((UserData)null);

            A.CallTo(() => _recipesRepository.GetMyRecipes(userModel.Id)).Returns((List < Recipe >) null);

            // Act
            var response = await _recipesServices.GetMyRecipes();

            // Assert
            Assert.Equal("401", response.Status);
            expectedRes.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task UpdateRecipe_ReturnsRecipeUpdated_Found()
        {
            // Arrange
            var oldRecipe = A.Fake<Recipe>();
            var expectedRecipe = A.Fake<Recipe>();

            A.CallTo(() => _recipeRepository.Update(oldRecipe)).Returns(expectedRecipe);

            // Act
            var response = await _recipesServices.Update(oldRecipe);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedRecipe, response.Data);
        }
        [Fact]
        public async Task UpdateRecipe_ReturnsRecipeUpdated_NotFound()
        {
            // Arrange
            var oldRecipe = A.Fake<Recipe>();
            var expectedRecipe = new { Title = "Failed" };

            A.CallTo(() => _recipeRepository.Update(oldRecipe)).Returns((Recipe)null);

            // Act
            var response = await _recipesServices.Update(oldRecipe);

            // Assert
            Assert.Equal("404", response.Status);
            expectedRecipe.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task DeleteRecipe_ReturnsRecipeUpdated_Found()
        {
            // Arrange
            var oldRecipe = A.Fake<Recipe>();
            var expectedRecipe = new { Title = "Deleted" };

            A.CallTo(() => _recipeRepository.Delete(oldRecipe));

            // Act
            var response = await _recipesServices.Delete(oldRecipe);

            // Assert
            Assert.Equal("200", response.Status);
            expectedRecipe.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task PostRecipe_ReturnsRecipeOfUser_Unauth()
        {
            // Arrange
            var recipe = A.Fake<Recipe>();
            var expectedRes = new { Title = "Untheorized User" };
            A.CallTo(() => _userService.GetMe()).Returns((UserData)null);

            A.CallTo(() => _recipeRepository.Create(recipe));
            var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
            var imageStream = new MemoryStream(imageContent);
            var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Act
            var response = await _recipesServices.AddRecipe(imageFile, recipe);

            // Assert
            Assert.Equal("401", response.Status);
            expectedRes.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task PostRecipe_ReturnsRecipeOfUser_Unauth_NotTheLoginedUser()
        {
            // Arrange
            var user1 = new UserData
            {
                Id=1,
                Username="Test",
            };
            var recipe = new Recipe
            {
                Id = 1,
                Title = "Test Recipe",
                Description = "Test description",
                Steps = "Test steps",
                Category = 1,
                CreatedBy = 2,
                TotalRating = 0,
                ImageFile = "test.jpg"
            };
            var expectedRes = new { Title = "Unauthorize user" };
            A.CallTo(() => _userService.GetMe()).Returns(user1);

            A.CallTo(() => _recipeRepository.Create(recipe));
            var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
            var imageStream = new MemoryStream(imageContent);
            var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Act
            var response = await _recipesServices.AddRecipe(imageFile, recipe);

            // Assert
            Assert.Equal("401", response.Status);
            expectedRes.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task PostRecipe_ReturnsRecipeCreated_auth_Created()
        {
            // Arrange
            var user1 = A.Fake<UserData>();
            var recipe = A.Fake<Recipe>();
            
            A.CallTo(() => _userService.GetMe()).Returns(user1);

            A.CallTo(() => _recipeRepository.Create(recipe)).Returns(recipe);
            
            // Act
            var response = await _recipesServices.AddRecipe((IFormFile)null, recipe);

            // Assert
            Assert.Equal("201", response.Status);
            Assert.Equal(recipe, response.Data);
        }
    }
}
