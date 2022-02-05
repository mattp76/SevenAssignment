using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.Helpers;
using SevenAssignmentLibrary.Models;
using SevenAssignmentLibrary.Services;
using SevenAssignmentLibrary.Settings;
using Xunit;

namespace SevenAssignment.Tests.Unit    
{
    public class SevenAssignmentHelperTests
    {
        private readonly ISevenAssignmentHelper _sevenAssignmentHelper;
        private readonly ICacheService _cacheService;
        private readonly ISevenClient _sevenClient;

        public SevenAssignmentHelperTests()
        {
            var settings = Substitute.For<SevenAssignmentSettings>();
            _cacheService = Substitute.For<ICacheService>();
            _sevenClient = Substitute.For<ISevenClient>();

            _sevenAssignmentHelper = new SevenAssignmentHelper(settings, _cacheService, _sevenClient);
        }

        [Fact]
        public async Task Should_Get_Users_From_Client()
        {
            _cacheService.Get<IList<UserModel>>(Arg.Any<string>()).Returns((List<UserModel>)null);
            _sevenClient.GetUsersAsync().Returns(new List<UserModel>() { new UserModel() {Age = 123, First = "Brian"}});

            var result = await _sevenAssignmentHelper.GetUsersAsync();

            _cacheService.Received(1).Get<List<UserModel>>(Arg.Any<string>());
            await _sevenClient.Received(1).GetUsersAsync();
            _cacheService.Received(1).Set(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<int>());


            result.Count.Should().Be(1);
            result[0].Age.Should().Be(123);
            result[0].First.Should().Be("Brian");
        }

        [Fact]
        public async Task Should_Get_Users_From_Cache()
        {
            _cacheService.Get<List<UserModel>>(Arg.Any<string>()).Returns(new List<UserModel>() { new UserModel() { Age = 123, First = "Cached Brian" } });
            
            var result = await _sevenAssignmentHelper.GetUsersAsync();

            _cacheService.Received(1).Get<List<UserModel>>(Arg.Any<string>());
            await _sevenClient.Received(0).GetUsersAsync();
            _cacheService.Received(0).Set(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<int>());

            result.Count.Should().Be(1);
            result[0].Age.Should().Be(123);
            result[0].First.Should().Be("Cached Brian");
        }

        [Fact]
        public void Should_Get_Gender_Count()
        {
            var list = new List<UserModel>()
            {
                new UserModel() {Age = 123, First = "Brian1", Gender = "M"},
                new UserModel() {Age = 123, First = "Brian2", Gender = "M"},
                new UserModel() {Age = 345, First = "Brian3", Gender = "M"},
                new UserModel() {Age = 345, First = "Brian4", Gender = "F"}
            };

            var resultMale = _sevenAssignmentHelper.GetGenderCount(list, "male");

            resultMale.Should().Be(3);

            var resultFemale = _sevenAssignmentHelper.GetGenderCount(list, "female");

            resultFemale.Should().Be(1);
        }
    }
}
