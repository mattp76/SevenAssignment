using System.Threading.Tasks;
using FluentAssertions;
using SevenAssignmentLibrary.DI;
using SevenAssignmentLibrary.Services;
using Xunit;

namespace SevenAssignment.Tests.Integration
{
    public class CacheServiceTests
    {
        private readonly ICacheService _cacheService;

        public CacheServiceTests()
        {
            DependencyResolver.ConfigureDependency();

            _cacheService = DependencyResolver.GetService<ICacheService>();
        }

        [Fact]
        public async Task Should_set_cache()
        {
             _cacheService.Set("key", "this is my object", 10);

            var result =  _cacheService.Get<string>("key");

            result.Should().Be("this is my object");
        }


        [Fact]
        public async Task Should_return_null_if_cache_does_not_exist()
        {
            var result = _cacheService.Get<string>("key123");

            result.Should().BeNull();
        }
    }
}
