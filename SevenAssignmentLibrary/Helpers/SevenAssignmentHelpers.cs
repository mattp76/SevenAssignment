using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SevenAssignmentLibrary.Clients;
using SevenAssignmentLibrary.Extensions;
using SevenAssignmentLibrary.Models;
using SevenAssignmentLibrary.Services;
using SevenAssignmentLibrary.Settings;

namespace SevenAssignmentLibrary.Helpers
{
    public class SevenAssignmentHelper : ISevenAssignmentHelper
    {
        private readonly SevenAssignmentSettings _sevenAssignmentSettings;
        private readonly ICacheService _cacheService;
        private readonly ISevenClient _sevenClient;

        public SevenAssignmentHelper(SevenAssignmentSettings sevenAssignmentSettings, 
            ICacheService cacheService,
            ISevenClient sevenClient)
        {
            _sevenAssignmentSettings = sevenAssignmentSettings;
            _cacheService = cacheService;
            _sevenClient = sevenClient;
        }

        public int GetGenderCount(IEnumerable<UserModel> grouping, string gender)
        {
            return grouping.Select(x => x.Gender.GetGenderFullName())
                .Where(x => x.ToLower() == gender)
                .GroupBy(x => x)
                .Select(y => y.Count()).FirstOrDefault();
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var cacheKey = _sevenAssignmentSettings.CacheKey;

            var users = _cacheService.Get<List<UserModel>>(cacheKey);

            //if no users in cache, get from client and then add to cache
            if (users == null)
            {
                users = await _sevenClient.GetUsersAsync();
                _cacheService.Set(cacheKey, users, _sevenAssignmentSettings.CacheExpireMins);
            }

            return users;
        }
    }
}
