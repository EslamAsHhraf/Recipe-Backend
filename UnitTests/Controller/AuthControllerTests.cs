using Authorization.Controllers;
using Authorization.Model;

using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Model;
using FakeItEasy;
using Firebase.Auth;
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

            var fakeResponse = new Response { Status = "400", Data = expectedData };

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

            var fakeResponse = new Response { Status = "404", Data = expectedData };
            A.CallTo(() => _userService.GetUserById(0)).Returns(null);

            // Act
            ActionResult<Response> result = await controller.GetUserById(0) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("404", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
        [Fact]
        public async void GetUserById_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);

            Tuple<string, int, string> myTuple = Tuple.Create("Hello", 42, "image");
            var expectedData = new
            {
                name = myTuple.Item1,
                id = myTuple.Item2,
                image = myTuple.Item3
            }; ;

            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.GetUserById(0)).Returns(myTuple);

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
        public async void GetMe_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);


            var user = new UserData
            {
                Id = 1,
                Name = "Eslam",
                ImageFile = "imageFile",
            };
            var expectedData = user;
            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.WhoLogin()).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = await controller.GetMe() as ActionResult<Response>;
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
        public async void GetMe_Should_Return_BadRequest()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Token not found" };
            var fakeResponse = new Response { Status = "401", Data = expectedData };
            A.CallTo(() => _userService.WhoLogin()).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = await controller.GetMe() as ActionResult<Response>;
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
        public async void Logout_Should_Return_BadRequest()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Token not found" };
            var fakeResponse = new Response { Status = "401", Data = expectedData };
            A.CallTo(() => _userService.logout()).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = controller.Logout() as ActionResult<Response>;
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
        public async void Logout_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Token Deleted successfully" };
            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.logout()).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = controller.Logout() as ActionResult<Response>;
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
        public async void Register_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);

            var loginModel = new UserDto
            {
                Username = "Eslam",
                Password = "100%Eslam"
            };
            var expectedData = new { Title = "User created" };
            var fakeResponse = new Response { Status = "201", Data = expectedData };
            A.CallTo(() => _userService.Register(loginModel)).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = controller.Register(loginModel) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("201", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }

        [Fact]
        public async void Register_Should_Return_BadRequest_When_Invalid_Input()
        {
            // Arrange

            controller = new AuthController(_userService);

            var loginModel = new UserDto
            {
                Username = "Eslam",
            };
            var expectedData = new { Title = "User Name and Password are required" };
            var fakeResponse = new Response { Status = "400", Data = expectedData };

            // Act
            ActionResult<Response> result = controller.Register(loginModel) as ActionResult<Response>;
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
        public async void ChangePassword_Should_Return_BadRequest_When_Invalid_Input()
        {
            // Arrange

            controller = new AuthController(_userService);

            string oldPassword = "";
            var expectedData = new { Title = "oldPassword and newPassword are required" };
            var fakeResponse = new Response { Status = "400", Data = expectedData };

            // Act
            ActionResult<Response> result = controller.ChangePassword(oldPassword, null) as ActionResult<Response>;
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
        public async void ChangePassword_Should_Return_OK()
        {
            // Arrange

            controller = new AuthController(_userService);

            string oldPassword = "5";
            string newPassword = "100%Eslam";
            var expectedData = new { Title = "Password change successfully" };
            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.changePassword(oldPassword, newPassword)).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = controller.ChangePassword(oldPassword, newPassword) as ActionResult<Response>;
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
        public async void UpdateImage_Should_Return_BadRequest_When_Invalid_Input()
        {
            // Arrange

            controller = new AuthController(_userService);

            var expectedData = new { Title = "ImageFile is null" };
            var fakeResponse = new Response { Status = "404", Data = expectedData };

            // Act
            ActionResult<Response> result = await controller.UpdateImage(null) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("404", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }
        [Fact]
        public async void UpdateImage_Should_Return_OK()
        {
            // Arrange
            var imageContent = new byte[] { 0x01, 0x02, 0x03 }; // Replace with your image content
            var imageStream = new MemoryStream(imageContent);
            var imageFile = new FormFile(imageStream, 0, imageStream.Length, "imageFile", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            controller = new AuthController(_userService);

            var expectedData = new { Title = "Update Successfully" };
            var fakeResponse = new Response { Status = "200", Data = expectedData };
            A.CallTo(() => _userService.SaveImage(imageFile)).Returns(fakeResponse);

            // Act
            ActionResult<Response> result = await controller.UpdateImage(imageFile) as ActionResult<Response>;
            // Assert
            result.Should().BeOfType<ActionResult<Response>>();

            var resultValue = (ObjectResult)result.Result;
            var responseData = (Response)resultValue.Value;
            // Access the Value from the ObjectResult
            Assert.NotNull(responseData);
            Assert.Equal("200", responseData.Status);
            fakeResponse.Should().BeEquivalentTo(responseData);
        }

    }

}
