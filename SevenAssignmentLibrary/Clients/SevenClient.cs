using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SevenAssignmentLibrary.Exceptions;
using SevenAssignmentLibrary.Models;
using SevenAssignmentLibrary.Settings;

namespace SevenAssignmentLibrary.Clients
{
    public class SevenClient : ISevenClient
    {
        private readonly HttpClient _httpClient;
        private readonly SevenAssignmentClientSettings _sevenAssignmentSettings;
        private readonly ILogger<ISevenClient> _logger;

        public SevenClient(HttpClient httpClient, SevenAssignmentClientSettings sevenAssignmentSettings, ILogger<ISevenClient> logger)
        {
            _httpClient = httpClient;
            _sevenAssignmentSettings = sevenAssignmentSettings;
            _logger = logger;
        }

        /// <summary>
        /// Get the users from the service end point
        /// </summary>
        public async Task<List<UserModel>> GetUsersAsync()
        {
            var url = $"{_sevenAssignmentSettings.ServiceUrl}/{_sevenAssignmentSettings.EndPoint}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<UserModel>>(data);
            }

            var error = "[SevenClient][GetUsersAsync] Could not get users. " +
                        $"Requested Url: '{url}'" +
                        $"Status:{response.StatusCode}, " +
                        $"Reason:{response.ReasonPhrase}";

            _logger.LogError(error);

            throw new SevenAssignmentException(error);
        }
    }
}
