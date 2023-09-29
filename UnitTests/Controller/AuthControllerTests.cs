using Authorization.Controllers;
using Authorization.Model;

using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace UnitTests.Controller
{
    public class AuthControllerTests
    {
        private readonly IAuthService _userService;
        private AuthController controller;

        private class UserObject
        {
            public UserData User { get; set; }
            public string Image { get; set; }
        }
        public AuthControllerTests()
        {
            _userService = A.Fake<IAuthService>();

        }

        [Fact]
        public async void Login_Should_Return_OK_When_Authentication_Successful()
        {
            // Arrange

            controller = new AuthController(_userService);

            var loginModel = new UserDto
            {
                Username = "Eslam",
                Password = "100%Eslam"
            };
            var image = new byte[] { 0x01, 0x02, 0x03 }; ;
            var expectedData = new UserObject
            {
                User = new UserData
                {
                    Id = 1,
                    Name = "Eslam",
                    ImageFile = "1659472095234252764.jpg"
                },
                Image = image.ToString()
            };
            var fakeResponse = new Response { Status = "200", Data = expectedData };

            A.CallTo(() => _userService.Login(loginModel)).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = await controller.Login(loginModel) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();


            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("200", responseData.Status);
            Assert.Equal(expectedData, responseData.Data);
        }
        [Fact]
        public async void Login_Should_Return_BadRequest_When_Invalid_Request()
        {
            // Arrange

            controller = new AuthController(_userService);
 
            var expectedData = new { Title = "User Name and Password are required" };

            var fakeResponse = new Response { Status = "400", Data=expectedData };

            // Act
            ActionResult<Response> result = await controller.Login(null) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("400", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
        [Fact]
        public async void GetUserById_Should_Return_BadRequest_When_Invalid_Id()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Cannot find user" };

            var fakeResponse = new Response { Status = "401", Data = expectedData };
            A.CallTo(() => _userService.GetUserById(0)).Returns(null);

            // Act
            ActionResult<Response> result = await controller.GetUserById(0) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("401", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
        [Fact]
        public async void GetUserById_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);

            Tuple<string, int> myTuple = Tuple.Create("Hello", 42);
            Byte[] imageUser = new byte[] { 0x01, 0x02, 0x03 }; ;
            var imageStream = new MemoryStream(imageUser);
            var expectedData = new
            {
                user = myTuple,
                image = imageStream
            };
            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.GetUserById(0)).Returns(myTuple);
            A.CallTo(() => _userService.GetImage(myTuple.Item1)).Returns(imageUser);

            // Act
            ActionResult<Response> result = await controller.GetUserById(0) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("200", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
        [Fact]
        public async void GetUserById_Should_Return_BadRequest_When_Invalid_Image()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Error in find image" };

            var fakeResponse = new Response { Status = "401", Data = expectedData };
            Tuple<string, int> myTuple = Tuple.Create("Hello", 42);
            Byte[] imageUser = null;
            A.CallTo(() => _userService.GetUserById(0)).Returns(myTuple);
            A.CallTo(() => _userService.GetImage(myTuple.Item1)).Returns(imageUser);

            // Act
            ActionResult<Response> result = await controller.GetUserById(0) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("401", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
    }

}

