using Authorization.Model;
using Business_Access_Layer.Abstract;
using Business_Access_Layer.Common;
using Business_Access_Layer.Concrete;
using DomainLayer.Interfaces;
using DomainLayer.Model;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace UnitTests.Business
{
    public class AuthBusinessTests
    {
        private AuthManager _userService;
        private readonly IUserRepository<User> _userRepository;
        private readonly IFileServices _fileServices;
        public readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthBusinessTests()
        {
            _userRepository = A.Fake<IUserRepository<User>>();
            _fileServices = A.Fake<IFileServices>();
            _configuration = A.Fake<IConfiguration>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();

        }

        [Fact]
        public async void Check_User_Return_OK()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);
            var userModel = new UserDto
            {
                Username = "Eslam",
                Password = "100%Eslam"
            };
            string HashString = "A092BACD405F9CA677415B75D0BB67E6253ED2054CFB75844C3C8311CF1C04C8A8B8561B5173E96ABEE46B242F482AF52E75D8726E8C3EFE352989E3B28332AC";
            byte[] PasswordHash = new byte[HashString.Length / 2];

            for (int i = 0; i < PasswordHash.Length; i++)
            {
                PasswordHash[i] = Convert.ToByte(HashString.Substring(i * 2, 2), 16);
            }
            string salatString = "BE0B6CC3958A844443F2A112FE4F8555FD813C95FE0CEBD576D498FCDA5E3255EEAB5D6FD0B603D959CC5FB71169A2C0BD0D54EFD1215CD4F1228B12466CAC897A27717856DDF7A6B03AEF7292FD5405DD0B8B16440F3B4A6F909AB5AD9DC014D81EC9567DAA2EFF4B3BA0B32D1B2F3BE58083BDE4C51567C9C7E9802D21FD6A";
            byte[] PasswordSalat = new byte[salatString.Length / 2];

            for (int i = 0; i < PasswordSalat.Length; i++)
            {
                PasswordSalat[i] = Convert.ToByte(salatString.Substring(i * 2, 2), 16);
            }


            var fakeResponse = new User
            {
                Id = 0,
                Username = "Eslam",
                PasswordSalt = PasswordSalat,
                PasswordHash = PasswordHash,
                ImageFile = "",

            };
            A.CallTo(() => _userRepository.GetUser(userModel.Username)).Returns(fakeResponse);

            // Act
            User result = await _userService.CheckUser(userModel);
            // Assert
            result.Should().BeOfType <User>();

            Assert.Equal(result, fakeResponse);
        }

        [Fact]
        public async void Check_User_Return_NULL_When_Password_Doesnot_Match()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);
            var userModel = new UserDto
            {
                Username = "Eslam",
                Password = "100Eslam"
            };
            string HashString = "A092BACD405F9CA677415B75D0BB67E6253ED2054CFB75844C3C8311CF1C04C8A8B8561B5173E96ABEE46B242F482AF52E75D8726E8C3EFE352989E3B28332AC";
            byte[] PasswordHash = new byte[HashString.Length / 2];

            for (int i = 0; i < PasswordHash.Length; i++)
            {
                PasswordHash[i] = Convert.ToByte(HashString.Substring(i * 2, 2), 16);
            }
            string salatString = "BE0B6CC3958A844443F2A112FE4F8555FD813C95FE0CEBD576D498FCDA5E3255EEAB5D6FD0B603D959CC5FB71169A2C0BD0D54EFD1215CD4F1228B12466CAC897A27717856DDF7A6B03AEF7292FD5405DD0B8B16440F3B4A6F909AB5AD9DC014D81EC9567DAA2EFF4B3BA0B32D1B2F3BE58083BDE4C51567C9C7E9802D21FD6A";
            byte[] PasswordSalat = new byte[salatString.Length / 2];

            for (int i = 0; i < PasswordSalat.Length; i++)
            {
                PasswordSalat[i] = Convert.ToByte(salatString.Substring(i * 2, 2), 16);
            }


            var fakeResponse = new User
            {
                Id = 0,
                Username = "Eslam",
                PasswordSalt = PasswordSalat,
                PasswordHash = PasswordHash,
                ImageFile = "",

            };
            A.CallTo(() => _userRepository.GetUser(userModel.Username)).Returns(fakeResponse);

            // Act
            User result = await _userService.CheckUser(userModel);
            // Assert

            Assert.Equal(result, null); ;
        }
        [Fact]
        public async void Check_User_Return_NULL_When_User_Doesnot_Found()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);
            var userModel = new UserDto
            {
                Username = "Eslam",
                Password = "100Eslam"
            };

            User fakeResponse = null;
           
            A.CallTo(() => _userRepository.GetUser(userModel.Username)).Returns(fakeResponse);

            // Act
            User result = await _userService.CheckUser(userModel);
            // Assert

            Assert.Equal(result, null); ;
        }

        [Fact]
        public async void Login_Return_Bad_Request_When_Input_Isnot_Valid()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);
            var userModel = new UserDto
            {
                Username = "Eslam",
                Password = "100Eslam"
            };
            string HashString = "A092BACD405F9CA677415B75D0BB67E6253ED2054CFB75844C3C8311CF1C04C8A8B8561B5173E96ABEE46B242F482AF52E75D8726E8C3EFE352989E3B28332AC";
            byte[] PasswordHash = new byte[HashString.Length / 2];

            for (int i = 0; i < PasswordHash.Length; i++)
            {
                PasswordHash[i] = Convert.ToByte(HashString.Substring(i * 2, 2), 16);
            }
            string salatString = "BE0B6CC3958A844443F2A112FE4F8555FD813C95FE0CEBD576D498FCDA5E3255EEAB5D6FD0B603D959CC5FB71169A2C0BD0D54EFD1215CD4F1228B12466CAC897A27717856DDF7A6B03AEF7292FD5405DD0B8B16440F3B4A6F909AB5AD9DC014D81EC9567DAA2EFF4B3BA0B32D1B2F3BE58083BDE4C51567C9C7E9802D21FD6A";
            byte[] PasswordSalat = new byte[salatString.Length / 2];

            for (int i = 0; i < PasswordSalat.Length; i++)
            {
                PasswordSalat[i] = Convert.ToByte(salatString.Substring(i * 2, 2), 16);
            }


            var userData = new User
            {
                Id = 0,
                Username = "Eslam",
                PasswordSalt = PasswordSalat,
                PasswordHash = PasswordHash,
                ImageFile = "image",

            };
            var expectedData = new { Title = "Password must include uppercase and lowercase and digit and special char and min length 8" };
            var fakeResponse = new Response { Status = "400", Data = expectedData };
            A.CallTo(() => _userRepository.GetUser(userModel.Username)).Returns((User)null);
            A.CallTo(() =>  _userRepository.Create(It.IsAny<User>())).Returns(true);
          
            // Act
            Response result = await  _userService.Register(userModel);
            // Assert
            result.Should().BeOfType<Response>();

            Assert.NotNull(result);
            Assert.Equal("400", result.Status);
            fakeResponse.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async void Login_Return_Bad_Request_When_User_Exists()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);
            var userModel = new UserDto
            {
                Username = "Eslam",
                Password = "100%Eslam"
            };
            string HashString = "A092BACD405F9CA677415B75D0BB67E6253ED2054CFB75844C3C8311CF1C04C8A8B8561B5173E96ABEE46B242F482AF52E75D8726E8C3EFE352989E3B28332AC";
            byte[] PasswordHash = new byte[HashString.Length / 2];

            for (int i = 0; i < PasswordHash.Length; i++)
            {
                PasswordHash[i] = Convert.ToByte(HashString.Substring(i * 2, 2), 16);
            }
            string salatString = "BE0B6CC3958A844443F2A112FE4F8555FD813C95FE0CEBD576D498FCDA5E3255EEAB5D6FD0B603D959CC5FB71169A2C0BD0D54EFD1215CD4F1228B12466CAC897A27717856DDF7A6B03AEF7292FD5405DD0B8B16440F3B4A6F909AB5AD9DC014D81EC9567DAA2EFF4B3BA0B32D1B2F3BE58083BDE4C51567C9C7E9802D21FD6A";
            byte[] PasswordSalat = new byte[salatString.Length / 2];

            for (int i = 0; i < PasswordSalat.Length; i++)
            {
                PasswordSalat[i] = Convert.ToByte(salatString.Substring(i * 2, 2), 16);
            }


            var userData = new User
            {
                Id = 0,
                Username = "Eslam",
                PasswordSalt = PasswordSalat,
                PasswordHash = PasswordHash,
                ImageFile = "image",

            };
            var expectedData = new { Title = "User already exists, please try different user name" };
            var fakeResponse = new Response { Status = "400", Data = expectedData };
            A.CallTo(() => _userRepository.GetUser(userModel.Username)).Returns(userData);


            // Act
            Response result = await _userService.Register(userModel);
            // Assert
            result.Should().BeOfType<Response>();

            Assert.NotNull(result);
            Assert.Equal("400", result.Status);
            fakeResponse.Should().BeEquivalentTo(result);
        }

        [Theory]
        [InlineData("P@ssw0rd", true)]  // Valid password
        [InlineData("Weak", false)]      // Invalid password
        [InlineData("Strong123!", true)] // Valid password
        [InlineData("Short!", false)]    // Invalid password
        public void CheckPasswordStrength_InlineData(string password, bool expectedResult)
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);

       

            // Act
            bool result = _userService.CheckPasswordStrength(password);

            // Assert
            Assert.Equal(expectedResult, result);
        }
        [Fact]
        public void MatchPasswordHash_PasswordsMatch_ReturnsTrue()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);

            string passwordText = "MyPassword";
            byte[] passwordKey = new byte[] { 1, 2, 3, 4, 5 }; // Example password key
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordKey))
            {
                var expectedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));

                // Act
                bool result = _userService.MatchPasswordHash(passwordText, expectedHash, passwordKey);

                // Assert
                Assert.True(result); // Expecting the result to be true since passwords match
            }
        }

        [Fact]
        public void MatchPasswordHash_PasswordsDoNotMatch_ReturnsFalse()
        {
            // Arrange
            _userService = new AuthManager(_userRepository, _configuration, _httpContextAccessor, _fileServices);

            string passwordText = "MyPassword";
            byte[] passwordKey = new byte[] { 1, 2, 3, 4, 5 }; // Example password key
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordKey))
            {
                var expectedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText + "Wrong")); // Change the password to make it incorrect

                // Act
                bool result = _userService.MatchPasswordHash(passwordText, expectedHash, passwordKey);

                // Assert
                Assert.False(result); // Expecting the result to be false since passwords do not match
            }
        }

    }
}
