using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Moq;
using RecipeAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using DomainLayer.Model;
using Business_Access_Layer.Concrete;
using FakeItEasy;
using FluentAssertions;

namespace UnitTests.Controller
{
    public class CategoryControllerTests
    {
        private readonly ICategory _category;
        public CategoryControllerTests()
        {
            _category = A.Fake<ICategory>();
        }
        [Fact]
        public async Task GetAllCategories_ReturnsExpectedData()
        {
            // Arrange
            var expectedData = new List<Category>
                {
                    new Category { Id = 1, Title = "Main Course" },
                    new Category { Id = 2, Title = "Main Course" },
                    new Category { Id = 3, Title = "Dessert" },
                    new Category { Id = 4, Title = "Side Dish" },
                    new Category { Id = 5, Title = "Appetizer" },
                    new Category { Id = 6, Title = "Drink" }
                };

            var fakeResponse = new Response { Status = "200", Data = expectedData };

            A.CallTo(() => _category.GetCategories()).Returns(fakeResponse);

            var controller = new CategoryController(_category);

            // Act
            var result = await controller.GetAllCategories();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            // Access the Value from the ObjectResult
            var resultValue = (ObjectResult)result.Result;
            resultValue.Value.Should().BeEquivalentTo(fakeResponse); // Check if the result.Value matches the expected response
                                                                     // Extract the data from the Response
            var responseData = (Response)resultValue.Value;
            responseData.Data.Should().BeOfType<List<Category>>(); // Check if the data is of type List<Category>
            responseData.Status.Should().BeEquivalentTo("200"); // Check if the data is of type List<Category>

        }

        [Fact]
        public async Task postCategory_ReturnsExpectedData()
        {
            // Arrange
            var data =
                    new Category { Id = 1, Title = "Main Course" };


            var fakeResponse = new Response { Status = "200", Data = data };

            A.CallTo(() => _category.PostCategory(data)).Returns(fakeResponse);

            var controller = new CategoryController(_category);

            // Act
            var result = await controller.postCategory(data);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ActionResult<Response>>();

            // Access the Value from the ObjectResult
            var resultValue = (ObjectResult)result.Result;
            resultValue.Value.Should().BeEquivalentTo(fakeResponse); // Check if the result.Value matches the expected response
                                                                     // Extract the data from the Response
            var responseData = (Response)resultValue.Value;
            responseData.Data.Should().BeOfType<Category>(); // Check if the data is of type List<Category>
            responseData.Status.Should().BeEquivalentTo("200"); // Check if the data is of type List<Category>

        }
    }

}