using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Business
{
    public class CategoryServicesTests
    {
        private CategoryServices _categoryServices;
        private readonly IRepository<Category> _categoryRepository;

        public CategoryServicesTests()
        {
            _categoryRepository = A.Fake<IRepository<Category>>();
            _categoryServices = new CategoryServices(_categoryRepository);
        }

        [Fact]
        public async Task GetCategories_ReturnsCategories()
        {
            // Arrange
            var expectedCategories = new List<Category>
            {
                new Category { Id = 1, Title = "Category1" },
                new Category { Id = 2, Title = "Category2" }
            };
            A.CallTo(() => _categoryRepository.GetAll()).Returns(expectedCategories);

            // Act
            var response = await _categoryServices.GetCategories();

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedCategories, response.Data);
        }

        [Fact]
        public async Task GetCategories_ReturnsNoContent()
        {
            // Arrange
            var expectedData = new { Title = "No Content" };
            

            A.CallTo(() => _categoryRepository.GetAll()).Returns(null); // Simulate no categories found

            // Act
            var response = await _categoryServices.GetCategories();

            // Assert
            Assert.Equal("204", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);

        }

        [Fact]
        public async Task GetCategoryById_ReturnsCategory()
        {
            // Arrange
            int categoryId = 1;
            var expectedCategory = new Category { Id = categoryId, Title = "TestCategory" };
            A.CallTo(() => _categoryRepository.GetById(categoryId)).Returns(expectedCategory);

            // Act
            var response = await _categoryServices.GetCategoryById(categoryId);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(expectedCategory, response.Data);
        }

        [Fact]
        public async Task GetCategoryById_ReturnsNotFound()
        {
            // Arrange
            var expectedData = new { Title = "Not Found" };
            int categoryId = 1;
            A.CallTo(() => _categoryRepository.GetById(categoryId)).Returns((Category)null); // Simulate category not found

            // Act
            var response = await _categoryServices.GetCategoryById(categoryId);

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }
        [Fact]
        public async Task PostCategory_ReturnsCategory()
        {
            // Arrange
            var categoryToCreate = new Category { Title = "NewCategory" };
            A.CallTo(() => _categoryRepository.Create(categoryToCreate)).Returns(categoryToCreate);

            // Act
            var response = await _categoryServices.PostCategory(categoryToCreate);

            // Assert
            Assert.Equal("200", response.Status);
            Assert.Equal(categoryToCreate, response.Data);
        }

        [Fact]
        public async Task PostCategory_ReturnsNotFound()
        {
            // Arrange
            var expectedData = new { Title = "Not Found" };

            var categoryToCreate = new Category { Title = "NewCategory" };
            A.CallTo(() => _categoryRepository.Create(categoryToCreate)).Returns((Category)null); // Simulate category not created

            // Act
            var response = await _categoryServices.PostCategory(categoryToCreate);

            // Assert
            Assert.Equal("404", response.Status);
            expectedData.Should().BeEquivalentTo(response.Data);
        }
    }
}