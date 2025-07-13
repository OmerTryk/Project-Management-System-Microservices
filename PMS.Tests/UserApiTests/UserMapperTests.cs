using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using UserApi.Context;
using UserApi.Mappers;
using UserApi.Models;
using UserApi.Models.ViewModels;
using Xunit;

namespace PMS.Tests.UserApiTests
{
    public class UserMapperTests
    {
        private readonly Mock<IUserMapper> _mockMapper;
        private readonly UserDbContext _dbContext;

        public UserMapperTests()
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _dbContext = new UserDbContext(options);
            _mockMapper = new Mock<IUserMapper>();
        }

        [Fact]
        public void MaptoUser_WithValidData_ReturnsUser()
        {
            var userDto = new DtoUserUI
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                NickName = "johndoe",
                HashPassword = "SecureP@ss123"
            };

            var expectedUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                NickName = userDto.NickName
            };

            _mockMapper.Setup(m => m.MaptoUser(It.IsAny<DtoUserUI>()))
                .Returns(expectedUser);

            var result = _mockMapper.Object.MaptoUser(userDto);

            Assert.NotNull(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(userDto.FirstName, result.FirstName);
            Assert.Equal(userDto.LastName, result.LastName);
            Assert.Equal(userDto.Email, result.Email);
            Assert.Equal(userDto.NickName, result.NickName);
        }

        [Fact]
        public void MapToDto_WithValidUser_ReturnsDtoUserGet()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                NickName = "janesmith"
            };

            var expectedDto = new DtoUserGet
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                NickName = user.NickName
            };

            _mockMapper.Setup(m => m.MapToDto(It.IsAny<User>()))
                .Returns(expectedDto);

            var result = _mockMapper.Object.MapToDto(user);

            Assert.NotNull(result);
            Assert.Equal(user.FirstName, result.FirstName);
            Assert.Equal(user.LastName, result.LastName);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.NickName, result.NickName);
        }

        [Fact]
        public void MapToDtos_WithValidUsers_ReturnsDtoUserGetList()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "User1",
                    LastName = "Test1",
                    Email = "user1@example.com",
                    NickName = "user1"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "User2",
                    LastName = "Test2",
                    Email = "user2@example.com",
                    NickName = "user2"
                }
            };

            var expectedDtos = users.Select(u => new DtoUserGet
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                NickName = u.NickName
            }).ToList();

            _mockMapper.Setup(m => m.MapToDtos(It.IsAny<IEnumerable<User>>()))
                .Returns(expectedDtos);

            var result = _mockMapper.Object.MapToDtos(users);

            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count());
            
            var resultList = result.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                Assert.Equal(users[i].FirstName, resultList[i].FirstName);
                Assert.Equal(users[i].LastName, resultList[i].LastName);
                Assert.Equal(users[i].Email, resultList[i].Email);
                Assert.Equal(users[i].NickName, resultList[i].NickName);
            }
        }
    }
}