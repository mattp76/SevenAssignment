using System.Threading.Tasks;
using FluentAssertions;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.DI;
using Xunit;

namespace SevenAssignment.Tests.Integration
{
    public class SevenClientTests
    {
        private readonly ISevenClient _sevenClient;

        public SevenClientTests()
        {
            DependencyResolver.ConfigureDependency();

            _sevenClient = DependencyResolver.GetService<ISevenClient>();
        }

        [Fact]
        public async Task Should_Get_Users()
        {
            var result = await _sevenClient.GetUsersAsync();

            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(0);
        }
    }
}
