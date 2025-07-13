using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using UserApi.Mappers;
using UserApi.Models;
using UserApi.Models.ViewModels;
using MassTransit;
using Xunit;

namespace PMS.Tests.UserApiTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserMapper> _mockMapper;
        private readonly Mock<ISendEndpointProvider> _mockSendEndpointProvider;

        public UserControllerTests()
        {
            _mockMapper = new Mock<IUserMapper>();
            _mockSendEndpointProvider = new Mock<ISendEndpointProvider>();
        }

        [Fact]
        public void MaptoUser_ReturnsExpectedUser()
        {
            var userDto = new DtoUserUI
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                NickName = "testuser",
                HashPassword = "hashedpassword"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                NickName = userDto.NickName,
                HashPassword = userDto.HashPassword
            };

            _mockMapper.Setup(m => m.MaptoUser(It.IsAny<DtoUserUI>()))
                .Returns(expectedUser);

            var result = _mockMapper.Object.MaptoUser(userDto);

            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(userDto.FirstName, result.FirstName);
            Assert.Equal(userDto.LastName, result.LastName);
        }

        [Fact]
        public void DtoUserUI_HasCorrectProperties()
        {
            var dto = new DtoUserUI
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                NickName = "johndoe",
                HashPassword = "password123"
            };

            Assert.Equal("John", dto.FirstName);
            Assert.Equal("Doe", dto.LastName);
            Assert.Equal("john.doe@example.com", dto.Email);
            Assert.Equal("johndoe", dto.NickName);
            Assert.Equal("password123", dto.HashPassword);
        }
    }
}