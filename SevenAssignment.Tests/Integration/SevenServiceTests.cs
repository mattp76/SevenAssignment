using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.DI;
using SevenAssignmentLibrary.Models;
using SevenAssignmentLibrary.Services;
using Xunit;

namespace SevenAssignment.Tests.Integration
{
    public class SevenServiceTests
    {
        private readonly ISevenService _sevenService;
        private readonly ISevenClient _sevenClient;
        private readonly UserModel _testUser;

        public SevenServiceTests()
        {
            DependencyResolver.ConfigureDependency();

            _sevenService = DependencyResolver.GetService<ISevenService>();
            _sevenClient = DependencyResolver.GetService<ISevenClient>();

            //first, call the client to get a list of available users (so we test against one we know exists)
            var _users = _sevenClient.GetUsersAsync().GetAwaiter().GetResult();

            var user = _users.First(); 
            _testUser = new UserModel()
            {
                Age = user.Age,
                Id = user.Id,
                First = user.First,
                Last = user.Last,
                Gender = user.Gender
            };
        }

        [Fact]
        public async Task Should_Get_User_By_Id()
        {

            //now test our service logic
            var result = await _sevenService.GetUserByIdAsync(_testUser.Id);

            result.Should().NotBeNull();
            result.Age.Should().Be(_testUser.Age);
            result.First.Should().Be(_testUser.First);
            result.Last.Should().Be(_testUser.Last);
            result.Gender.Should().Be(_testUser.Gender);
            result.Id.Should().Be(_testUser.Id);
        }

        [Fact]
        public async Task Should_Return_Null_If_Id_Doesnt_Exists()
        {
            //now test our service logic
            var result = await _sevenService.GetUserByIdAsync(123452131);

            result.Should().BeNull();
        }
    }
}
