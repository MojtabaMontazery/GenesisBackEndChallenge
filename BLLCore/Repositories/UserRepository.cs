using DALCore;
using DALCore.DB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLLCore.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDataAccess dataAccess;
        private ILogger logger;

        public ILogger Logger
        {
            set
            {
                this.logger = value;
                if (dataAccess != null && dataAccess.Logger == null)
                    dataAccess.Logger = value;
            }
        }


        
        public UserRepository(IUserDataAccess dataAccess, ILogger logger = null)
        {
            this.dataAccess = dataAccess;
            if (logger == null)
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                logger = loggerFactory.CreateLogger<UserRepository>();
            }
            this.logger = logger;
        }

        /// <summary>
        /// Checks if the user already exists
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>A Task<bool> which is true if user exists.</returns>
        public async Task<bool> UserAlreadyExists(string email)
        {
            logger.LogInformation("Checking if a user already exists by email: {email}", email);
            Users user = await dataAccess.GetUserByEmailAsync(email);
            return user != null;
        }

        /// <summary>
        /// Create a user by provided parameters
        /// </summary>
        /// <param name="token">A JWT security token generated for this user</param>
        /// <param name="telephones">A string array containing the phone numbers</param>
        /// <returns></returns>
        public async Task<Users> CreateUser(string name, string email, string password, string token, string[] telephones)
        {
            logger.LogInformation("Creating an user object based on input parameters");
            Users user = new Users()
            {
                CreatedOn = DateTime.Now,
                Email = email,
                LastLoginOn = DateTime.Now,
                Name = name,
                Token = token,
                UserId = Guid.NewGuid(),
                PasswordHash= PasswordStorage.CreateHash(password),
                UsersPhones = telephones.Select(ent => new UsersPhones { Phone = ent }).ToList()
            };

            return await dataAccess.CreateUserAsync(user);

        }

        /// <summary>
        /// Sign in user
        /// </summary>
        /// <returns></returns>
        public async Task<Users> SignInUserAsync(string email, string password)
        {
            logger.LogInformation("The user signning in");
            Users user = await dataAccess.GetUserByEmailAsync(email);

            if (user == null)
            {
                logger.LogWarning("The user is not found to sign in, email: {email}", email);
                return null;
            }

            logger.LogInformation("Verifying password for the user");
            if (!PasswordStorage.VerifyPassword(password, user.PasswordHash))
            {
                logger.LogWarning("Password was not verified to sign in");
                return null;
            }

            return await dataAccess.UserLoggedIn(user.UserId);
        }

        /// <summary>
        /// Get user by Id.
        /// </summary>
        /// <returns></returns>
        public async Task<Users> GetUserByIdAsync(Guid id)
        {
            Users user = await dataAccess.GetUserByIdAsync(id);

            return user;
        }
    }
}
