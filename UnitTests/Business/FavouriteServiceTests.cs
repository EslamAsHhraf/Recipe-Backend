using Business_Access_Layer.Concrete;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using FakeItEasy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace UnitTests.Business
{
    public class FavouriteServiceTests
    {
        private FavouriteService _favouriteService;
        private readonly IRepository<Favourite> _favouriteRepository;
        private readonly IAuthService _userService;
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IFileServices _fileServices;

        public FavouriteServiceTests()
        {
            _favouriteRepository = A.Fake<IRepository<Favourite>>();
            _userService = A.Fake<IAuthService>();
            _recipeRepository = A.Fake<IRepository<Recipe>>();
            _fileServices = A.Fake<IFileServices>();
            _favouriteService = new FavouriteService(_favouriteRepository, _recipeRepository, _fileServices);
        }

        [Fact]
        public async Task GetMyFavourites_ReturnsUserFavourites()
        {
            // Arrange
            int userId = 1;
            var favouritesList = new List<Favourite>
            {
                new Favourite { Id = 1, AuthorId = userId },
                new Favourite { Id = 2, AuthorId = userId },
                new Favourite { Id = 3, AuthorId = 2 }, // Favourite of another user
            };
            A.CallTo(() => _favouriteRepository.GetAll()).Returns(favouritesList);

            // Act
            var response = await _favouriteService.GetMyFavourites(userId);

            // Assert
            Assert.Equal("200", response.Status);
            var data = response.Data as List<Favourite>;
            Assert.NotNull(data);
            Assert.Equal(2, data.Count); // Only favourites of the specified user are returned
        }

        [Fact]
        public async Task CreateFavourite_ReturnsCreatedFavourite()
        {
            // Arrange
            var favouriteToCreate = new Favourite { Id = 1, AuthorId = 1 };
            A.CallTo(() => _favouriteRepository.Create(favouriteToCreate)).Returns(favouriteToCreate);

            // Act
            var response = await _favouriteService.CreateFavourite(favouriteToCreate);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(favouriteToCreate, response.Data);
        }

        [Fact]
        public async Task DeleteFavourite_ReturnsDeleted()
        {
            // Arrange
            var expectedData = new { Title = "Deleted" };

            int favouriteId = 1;
            var favouriteToDelete = new Favourite { Id = favouriteId, AuthorId = 1 };
            A.CallTo(() => _favouriteRepository.GetById(favouriteId)).Returns(favouriteToDelete);

            // Act
            var response = await _favouriteService.DeleteFavourite(favouriteId);

            // Assert
            Assert.Equal("200", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }

        [Fact]
        public async Task DeleteFavourite_ReturnsNotFound()
        {
            // Arrange
            var expectedData = new { Title = "Not Created" };

            int favouriteId = 1;
            A.CallTo(() => _favouriteRepository.GetById(favouriteId)).Returns((Favourite)null); // Simulate not found

            // Act
            var response = await _favouriteService.DeleteFavourite(favouriteId);

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }

        [Fact]
        public async Task GetRecipesFavourite_ReturnsUserFavouriteRecipes()
        {
            // Arrange
            int userId = 1;
            var favouritesList = new List<Favourite>
            {
                new Favourite { Id = 1, AuthorId = userId, RecipeId = 1 },
                new Favourite { Id = 2, AuthorId = userId, RecipeId = 2 },
                new Favourite { Id = 3, AuthorId = 2, RecipeId = 3 }, // Favourite of another user
            };
            var allRecipes = new List<Recipe>
            {
                new Recipe { Id = 1, Title = "Recipe1" },
                new Recipe { Id = 2, Title = "Recipe2" },
                new Recipe { Id = 3, Title = "Recipe3" },
            };
            A.CallTo(() => _favouriteRepository.GetAll()).Returns(favouritesList);
            A.CallTo(() => _recipeRepository.GetAll()).Returns(allRecipes);

            // Act
            var response = await _favouriteService.GetRecipesFavourite(userId);

            // Assert
            Assert.Equal("200", response.Status);
            var data = response.Data as IEnumerable<Recipe>;
            Assert.NotNull(data);
            Assert.Equal(2, data.Count()); // Only recipes favourited by the specified user are returned
        }

        [Fact]
        public async Task GetRecipesFavourite_ReturnsNotFound()
        {
            // Arrange
            var expectedData = new { Title = "Not found" };

            int userId = 1;
            var favouritesList = new List<Favourite>
            {
                new Favourite { Id = 1, AuthorId = userId, RecipeId = 1 },
                new Favourite { Id = 2, AuthorId = userId, RecipeId = 2 },
            };
            var allRecipes = new List<Recipe>
            {
                new Recipe { Id = 3, Title = "Recipe3" },
                new Recipe { Id = 4, Title = "Recipe4" },
            };
            A.CallTo(() => _favouriteRepository.GetAll()).Returns(favouritesList);
            A.CallTo(() => _recipeRepository.GetAll()).Returns(allRecipes);

            // Act
            var response = await _favouriteService.GetRecipesFavourite(userId);

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);

        }

    }
}