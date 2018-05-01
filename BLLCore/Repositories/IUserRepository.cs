using System;
using System.Threading.Tasks;
using DALCore.DB;
using Microsoft.Extensions.Logging;

namespace BLLCore.Repositories
{
    public interface IUserRepository
    {
        ILogger Logger { set; }

        Task<Users> CreateUser(string name, string email, string password, string token, string[] telephones);
        Task<Users> GetUserByIdAsync(Guid id);
        Task<Users> SignInUserAsync(string email, string password);
        Task<bool> UserAlreadyExists(string email);
    }
}