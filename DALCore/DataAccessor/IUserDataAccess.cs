using System;
using System.Threading.Tasks;
using DALCore.DB;
using Microsoft.Extensions.Logging;

namespace DALCore
{
    public interface IUserDataAccess
    {
        ILogger Logger { get; set; }

        Task<Users> CreateUserAsync(Users user);
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> GetUserByIdAsync(Guid id);
        Task<Users> UserLoggedIn(Guid userId);
    }
}