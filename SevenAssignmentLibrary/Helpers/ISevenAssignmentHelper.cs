using System.Collections.Generic;
using System.Threading.Tasks;
using SevenAssignmentLibrary.Models;

namespace SevenAssignmentLibrary.Helpers
{
    public interface ISevenAssignmentHelper
    {
        int GetGenderCount(IEnumerable<UserModel> grouping, string gender);
        Task<List<UserModel>> GetUsersAsync();
    }
}