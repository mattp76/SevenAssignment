using System.Collections.Generic;
using System.Threading.Tasks;
using SevenAssignmentLibrary.Models;

namespace SevenAssignmentLibrary.Clients
{
    public interface ISevenClient
    {
        Task<List<UserModel>> GetUsersAsync();
    }
}