using System.Collections.Generic;
using System.Threading.Tasks;
using SevenAssignmentLibrary.Models;

namespace SevenAssignmentLibrary.Services
{
    public interface ISevenService
    {
        Task<UserModel> GetUserByIdAsync(int id);

        Task<string> GetCommaSeperatedUsersByAgeAsync(int age);

        Task<List<string>> GetNumberOfGendersPerAgeAsync();
    }
}