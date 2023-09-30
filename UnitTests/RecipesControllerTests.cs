using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using DomainLayer.Model;
using Microsoft.AspNetCore.Mvc;
using FakeItEasy;
using FluentAssertions;
using Xunit;
using RecipeAPI.Controllers;
using Business_Access_Layer.Concrete;
using Microsoft.AspNetCore.Http;
using PresistenceLayer.Repository;

namespace UnitTests
{
    public class RecipesControllerTests
    {/* 
        private readonly IRecipesServices _recipesServices;
        private readonly IAuthService _authService;
        private readonly IFileServices _fileServices;
        private readonly ICategory _category;
        private readonly IRecipeIngredientsService _recipeIngredientsService;
        private RecipesController controller;

       public RecipesControllerTests()
        {
            _recipesServices = A.Fake<IRecipesServices>();
            _authService = A.Fake<IAuthService>();
            _fileServices = A.Fake<IFileServices>();
            _category = A.Fake<ICategory>();
            _recipeIngredientsService = A.Fake<IRecipeIngredientsService>();

        }

        [Fact]
        public void GetAllRecipes_ReturnsExpectedData()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );
            var expectedData = A.Fake<IEnumerable<Recipe>>();

            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _recipesServices.GetAllRecipes()).Returns(fakeResponse);

            // Act
            var result = controller.GetAllRecipes();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            // Access the Value from the ObjectResult
            var resultValue = (ObjectResult)result.Result;
            resultValue.Value.Should().BeEquivalentTo(fakeResponse); // Check if the result.Value matches the expected response
                                                                     
        }

        //[Fact]
        //public void GetRecipeById_ReturnsExpectedData()
        //{
        //    // Arrange
        //    controller = new RecipesController(
        //        _recipesServices,
        //        _authService,
        //        _fileServices,
        //        _category,
        //        _recipeIngredientsService
        //    );

        //    int recipeId = 3; // Specify the recipe ID you want to test
        //    var expectedRecipe = new Recipe
        //    {
        //        Id = recipeId,
        //        Title = "Test Recipe",
        //        Description = "This is a test recipe.",
        //        Steps = "1. Test step.",
        //        Category = 1,
        //        CreatedBy = 1,
        //        TotalRating = 5,
        //        ImageFile = "test.jpg"
        //    };

        //    var expectedIngredients = new List<RecipeIngredients>
        //    {
        //        new RecipeIngredients { Id = 1, RecipeId = recipeId, Title = "Chicken" },
        //        new RecipeIngredients { Id = 2, RecipeId = recipeId, Title = "Beef" },
        //    };

        //    var expectedCreatedBy = Tuple.Create("TestUser", 1);
        //    var expectedCategory = new Category { Id = 1, Title = "Main Course" };
        //    var expectedImage = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your expected image bytes

        //    var fakeRecipeResponse = new Response { Status = "200", Data = expectedRecipe };
        //    var fakeIngredientsResponse = new Response { Status = "200", Data = expectedIngredients };
        //    var fakeCategoryResponse = new Response { Status = "200", Data = expectedCategory };

        //    A.CallTo(() => _recipesServices.GetRecipeById(recipeId)).Returns(fakeRecipeResponse);
        //    A.CallTo(() => _recipeIngredientsService.GetRecipeIngredients(expectedRecipe)).Returns(fakeIngredientsResponse);
        //    A.CallTo(() => _authService.GetUserById(expectedRecipe.CreatedBy)).Returns(expectedCreatedBy);
        //    A.CallTo(() => _category.GetCategoryById(expectedRecipe.Category)).Returns(fakeCategoryResponse);
        //    A.CallTo(() => _fileServices.GetImage(expectedRecipe.ImageFile)).Returns(expectedImage);

        //    // Act
        //    var result = controller.GetRecipeById(recipeId);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType<ActionResult<Response>>();

        //}
        //[Fact]
        //public async Task PostRecipe_WithValidData_CreatesRecipeAndReturnsCreated()
        //{
        //    // Arrange
        //    controller = new RecipesController(
        //        _recipesServices,
        //        _authService,
        //        _fileServices,
        //        _category,
        //        _recipeIngredientsService
        //    );

        //    // Create a test IFormFile with a sample image content
        //    var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
        //    var imageStream = new MemoryStream(imageContent);
        //    var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
        //    {
        //        Headers = new HeaderDictionary(),
        //        ContentType = "image/jpeg"
        //    };

        //    // Create a recipe object with the necessary data
        //    var recipe = new Recipe
        //    {
        //        Id = 1,
        //        Title = "Test Recipe",
        //        Description = "Test description",
        //        Steps = "Test steps",
        //        Category = 1,
        //        CreatedBy = 1,
        //        TotalRating = 0,
        //        ImageFile = "test.jpg"
        //    };

        //    // Simulate an authenticated user
        //    var userData = new UserData { Id = 1 };
        //    A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

        //    // Simulate the recipe creation and save image process
        //    var createdRecipe = recipe; // Replace with your actual logic for creating the recipe
        //    var expectedResponse = new Response { Status = "201", Data = createdRecipe };
        //    A.CallTo(() => _recipesServices.SaveImageRecipe(A<IFormFile>._, A<Recipe>._)).Returns(Task.FromResult(createdRecipe));
        //    A.CallTo(() => _recipesServices.Create(A<Recipe>._)).Returns(Task.FromResult(expectedResponse));

        //    // Act
        //    var result = await controller.PostRecipe(imageFile, recipe);

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType<ObjectResult>();

        //    var objectResult = (ObjectResult)result;
        //    objectResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        //    objectResult.Value.Should().BeEquivalentTo(expectedResponse);
        //}
        [Fact]
        public async Task PostRecipe_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            // Create a test IFormFile with a sample image content
            var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
            var imageStream = new MemoryStream(imageContent);
            var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Simulate an unauthorized user (null user data)
            UserData userData = null;
            A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

            // Create a recipe object with the necessary data
            var recipe = new Recipe
            {
                Id = 1,
                Title = "Test Recipe",
                Description = "Test description",
                Steps = "Test steps",
                Category = 1,
                CreatedBy = 1,
                TotalRating = 0,
                ImageFile = "test.jpg"
            };

            // Act
            var result = await controller.PostRecipe(imageFile, recipe);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
        [Fact]
        public async Task PostRecipe_UserIsNotTheCreator_ReturnsUnauthorized()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            // Create a test IFormFile with a sample image content
            var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
            var imageStream = new MemoryStream(imageContent);
            var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Simulate an authenticated user
            var userData = new UserData { Id = 2 }; // User ID different from the recipe's creator
            A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

            // Create a recipe object with the necessary data
            var recipe = new Recipe
            {
                Id = 1,
                Title = "Test Recipe",
                Description = "Test description",
                Steps = "Test steps",
                Category = 1,
                CreatedBy = 1, // Recipe created by a different user
                TotalRating = 0,
                ImageFile = "test.jpg"
            };

            // Act
            var result = await controller.PostRecipe(imageFile, recipe);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();

            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
        [Fact]
        public async Task GetMyRecipes_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            // Simulate an unauthorized user (null user data)
            var fakeResponse = new Response { Status = "401", Data = null };

            A.CallTo(() => _recipesServices.GetMyRecipes()).Returns(fakeResponse);

            // Act
            var result = controller.GetMyRecipes();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            var objectResult = (ObjectResult)result.Result;

            var response = (Response)objectResult.Value;
            response.Status.Should().Be("401");
            response.Data.Should().Be(null);
        }
        [Fact]
        public async Task GetMyRecipes_AuthorizedUser_ReturnsOk()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            // Simulate an authenticated user
            var userData = new UserData { Id = 1 };
            A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

            // Create a list of recipes for the user
            var myRecipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 4,
                    Title = "Lasagna",
                    Description = "This is a hearty pasta dish made with layers of pasta, tomato sauce, and cheese. and have meat.",
                    Steps = "Cook lasagna noodles according to package directions.",
                    Category = 1,
                    CreatedBy = 1,
                    TotalRating = 2,
                    ImageFile = "las.jpg"
                },
            };
            var fakeResponse = new Response { Status = "200", Data = myRecipes };

            A.CallTo(() => _recipesServices.GetMyRecipes()).Returns(fakeResponse);

            // Act
            var result = controller.GetMyRecipes();

                // Assert
                result.Should().NotBeNull();
                result.Should().BeOfType<ActionResult<Response>>();

                var objectResult = (ObjectResult)result.Result;
                objectResult.Value.Should().NotBeNull();

                var response = (Response)objectResult.Value;
                response.Status.Should().Be("200");
                response.Data.Should().BeEquivalentTo(myRecipes);
        }
        [Fact]
        public async Task PutRecipe_WithValidData_UpdatesRecipeAndReturnsUpdated()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            int recipeId = 1; // Specify the recipe ID you want to test
            var expectedRecipe = new Recipe
            {
                Id = recipeId,
                Title = "Updated Recipe",
                Description = "Updated description",
                Steps = "Updated steps",
                Category = 2, // Updated category
                CreatedBy = 1,
                TotalRating = 5,
                ImageFile = "test.jpg"
            };

            // Simulate an authenticated user
            var userData = new UserData { Id = 1 };
            A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

            // Simulate the recipe update process
            A.CallTo(() => _recipesServices.GetRecipeById(recipeId)).Returns(new Response { Status = "200", Data = expectedRecipe });
            A.CallTo(() => _recipesServices.Update(A<Recipe>._)).Returns(new Response { Status = "200", Data = expectedRecipe });

            // Act
            var result = controller.PutRecipe(recipeId, expectedRecipe);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            var objectResult = (ObjectResult)result.Result;
            objectResult.Value.Should().NotBeNull();

            var response = (Response)objectResult.Value;
            response.Status.Should().Be("200");
            response.Data.Should().BeEquivalentTo(expectedRecipe);
        }
        [Fact]
        public async Task DeleteRecipe_WithValidId_DeletesRecipeAndReturnsNoContent()
        {
            // Arrange
            controller = new RecipesController(
                _recipesServices,
                _authService,
                _fileServices,
                _category,
                _recipeIngredientsService
            );

            int recipeId = 1; // Specify the recipe ID you want to test
            var expectedRecipe = new Recipe
            {
                Id = recipeId,
                Title = "Recipe to Delete",
                Description = "Description to Delete",
                Steps = "Steps to Delete",
                Category = 1,
                CreatedBy = 1,
                TotalRating = 5,
                ImageFile = "test.jpg"
            };

            // Simulate an authenticated user
            var userData = new UserData { Id = 1 };
            A.CallTo(() => _authService.GetMe()).Returns(Task.FromResult(userData));

            // Simulate the recipe deletion process
            A.CallTo(() => _recipesServices.GetRecipeById(recipeId)).Returns(new Response { Status = "200", Data = expectedRecipe });
            A.CallTo(() => _recipeIngredientsService.GetRecipeIngredients(A<Recipe>._)).Returns(new Response { Status = "200", Data = new List<RecipeIngredients>() });
            A.CallTo(() => _recipeIngredientsService.DeleteRecipeIngredients(A<IEnumerable<RecipeIngredients>>._)).Returns(new Response { Status = "204", Data = null });
            A.CallTo(() => _recipesServices.Delete(A<Recipe>._)).Returns(new Response { Status = "200", Data = "Deleted" });

            // Act
            var result = await controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            var objectResult = (ObjectResult)result.Result;
            objectResult.Value.Should().NotBeNull();

            var response = (Response)objectResult.Value;
            response.Status.Should().Be("200");
            response.Data.Should().BeEquivalentTo( "Deleted" );
        }

*/
    }
}
