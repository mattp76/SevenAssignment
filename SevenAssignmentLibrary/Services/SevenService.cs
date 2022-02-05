using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SevenAssignmentLibrary.Exceptions;
using SevenAssignmentLibrary.Helpers;
using SevenAssignmentLibrary.Models;

namespace SevenAssignmentLibrary.Services
{
    public class SevenService : ISevenService
    {
        private readonly ILogger<ISevenService> _logger;
        private readonly ISevenAssignmentHelper _sevenAssignmentHelper;

        public SevenService(ILogger<ISevenService> logger,
            ISevenAssignmentHelper sevenAssignmentHelper)
        {
            _logger = logger;
            _sevenAssignmentHelper = sevenAssignmentHelper;
        }

        /// <summary>
        /// Get the user by id
        /// </summary>
        /// <param name="id">The id</param>
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            try
            {
                var users = await _sevenAssignmentHelper.GetUsersAsync();
                return users.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[SevenService][GetUserByIdAsync] Exception {ex.Message}");
                throw new SevenAssignmentException($"[SevenService][GetUserByIdAsync] Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// All the users first names (comma separated) by age
        /// </summary>
        /// <param name="age">The age</param>
        public async Task<string> GetCommaSeperatedUsersByAgeAsync(int age)
        {
            try
            {
                var users = await _sevenAssignmentHelper.GetUsersAsync();
                var firstNameJoined = string.Join(",", users.Where(x => x.Age == age).Select(y => y.First));

                return firstNameJoined;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[SevenService][GetUserByIdAsync] Exception {ex.Message}");
                throw new SevenAssignmentException($"[SevenService][GetUserByIdAsync] Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// The number of genders per Age, displayed from youngest to oldest
        /// </summary>
        public async Task<List<string>> GetNumberOfGendersPerAgeAsync()
        {
            try
            {
                var users = await _sevenAssignmentHelper.GetUsersAsync();

                var ageGenderGrouping = users
                    .GroupBy(x => x.Age)
                    .Select(g => new
                {
                    age = g.Key,
                    femaleCount = _sevenAssignmentHelper.GetGenderCount(g, "female"),
                    maleCount = _sevenAssignmentHelper.GetGenderCount(g, "male")
                }).OrderBy(x => x.age);

                var genderCountList = new List<string>();

                foreach (var a in ageGenderGrouping)
                { 
                    genderCountList.Add($"Age: {a.age} Female: {a.femaleCount} Male: {a.maleCount}");
                }


                return genderCountList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"[SevenService][GetUserByIdAsync] Exception {ex.Message}");
                throw new SevenAssignmentException($"[SevenService][GetUserByIdAsync] Exception: {ex.Message}");
            }
        }
    }
}
