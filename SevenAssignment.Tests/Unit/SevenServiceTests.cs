using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using SevenAssignmentLibrary.Exceptions;
using SevenAssignmentLibrary.Helpers;
using SevenAssignmentLibrary.Models;
using SevenAssignmentLibrary.Services;
using Xunit;

namespace SevenAssignment.Tests.Unit
{
    public class SevenServiceTests
    {
        private readonly ISevenService _sevenService;
        private readonly ISevenAssignmentHelper _sevenAssignmentHelper;

        public SevenServiceTests()
        {
            _sevenAssignmentHelper = Substitute.For<ISevenAssignmentHelper>();
            var logger = Substitute.For<ILogger<ISevenService>>();

            _sevenService = new SevenService(logger, _sevenAssignmentHelper);
        }

        [Fact]
        public async Task Should_Get_single_user_By_Id()
        {
            var expectedUserList = new List<UserModel>();

            expectedUserList.Add(new UserModel()
            {
                Age = 12,
                Id = 1,
                First = "Jackson",
                Last = "Storm",
                Gender = "Male"
            });
            expectedUserList.Add(new UserModel()
            {
                Age = 15,
                Id = 2,
                First = "Lightning",
                Last = "McQueen",
                Gender = "Male"
            });

            _sevenAssignmentHelper.GetUsersAsync().Returns(expectedUserList);

            //now test our service logic
            var result = await _sevenService.GetUserByIdAsync(1);

            result.Should().NotBeNull();
            result.Age.Should().Be(12);
            result.First.Should().Be("Jackson");
            result.Last.Should().Be("Storm");

        }

        [Fact]
        public async Task Should_Not_Get_User_And_Throw_Exception_If_Users_Is_Null()
        {
            _sevenAssignmentHelper.GetUsersAsync().Returns((List<UserModel>)null);

            await Assert.ThrowsAsync<SevenAssignmentException>(() => _sevenService.GetUserByIdAsync(123));
        }

        [Fact]
        public async Task Should_Return_Comma_Separated_List_Users()
        {
            var dummyResponse = File.ReadAllText("./TestData/Response.json");
            var users = JsonConvert.DeserializeObject<List<UserModel>>(dummyResponse);

            _sevenAssignmentHelper.GetUsersAsync().Returns(users);

            //now test our service logic against age 23
            var result = await _sevenService.GetCommaSeperatedUsersByAgeAsync(23);

            result.Should().NotBeNull();
            result.Should().Be("Bill,Frank");
        }

        [Fact]
        public async Task Should_Not_Get_CommaSeperated_Names_And_Throw_Exception_If_Users_Is_Null()
        {
            _sevenAssignmentHelper.GetUsersAsync().Returns((List<UserModel>)null);

            await Assert.ThrowsAsync<SevenAssignmentException>(() => _sevenService.GetCommaSeperatedUsersByAgeAsync(123));
        }

        [Fact]
        public async Task Should_Return_Number_Of_Genders_Per_Age()
        {
            var dummyResponse = File.ReadAllText("./TestData/Response.json");
            var users = JsonConvert.DeserializeObject<List<UserModel>>(dummyResponse);

            _sevenAssignmentHelper.GetUsersAsync().Returns(users);
            _sevenAssignmentHelper.GetGenderCount(Arg.Any<IEnumerable<UserModel>>(), "female").Returns(1);
            _sevenAssignmentHelper.GetGenderCount(Arg.Any<IEnumerable<UserModel>>(), "male").Returns(3);

            //now test our service logic
            var result = await _sevenService.GetNumberOfGendersPerAgeAsync();

            result.Any().Should().BeTrue();
            result.Count.Should().Be(3);
            result[0].Should().Be($"Age: 23 Female: 1 Male: 3");
            result[1].Should().Be($"Age: 54 Female: 1 Male: 3");
            result[2].Should().Be($"Age: 66 Female: 1 Male: 3");
        }

        [Fact]
        public async Task Should_Not_Return_Number_Of_Genders_Per_And_Throw_Exception_If_Users_Is_Null()
        {
            _sevenAssignmentHelper.GetUsersAsync().Returns((List<UserModel>)null);

            await Assert.ThrowsAsync<SevenAssignmentException>(() => _sevenService.GetNumberOfGendersPerAgeAsync());
        }

    }
}
