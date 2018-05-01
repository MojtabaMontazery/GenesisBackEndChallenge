using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DALCore.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DALCore
{
    public class UserDataAccess : IUserDataAccess
    {
        private readonly GenesisChallengeContext db;
        private ILogger logger;

        public ILogger Logger
        {
            get { return logger; }
            set
            {
                this.logger = value;
            }
        }

        public UserDataAccess(GenesisChallengeContext db, ILogger logger=null)
        {
            this.db = db;
            if (logger == null)
            {
                ILoggerFactory loggerFactory = new LoggerFactory();
                logger = loggerFactory.CreateLogger<UserDataAccess>();
            }
            this.logger = logger;
        }

        /// <summary>
        /// Get the user by email
        /// </summary>
        /// <returns></returns>
        public async Task<Users> GetUserByEmailAsync(string email)
        {
            logger.LogInformation("Getting user by email: {email}", email);
            try
            {
                return await db.Users.Where(ent => ent.Email == email).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in getting user by email: {email}", email);
                logger.LogDebug("Exception message: {message}", ex.Message);
                logger.LogTrace("Exception stack trace: {trace}", ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Create the user
        /// </summary>
        /// <returns></returns>
        public async Task<Users> CreateUserAsync(Users user)
        {
            logger.LogInformation("Adding the user to the db context");
            db.Users.Add(user);
            try
            {
                logger.LogInformation("Saving db change after adding the user");
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in inserting the user");
                logger.LogDebug("Exception message: {message}", ex.Message);
                logger.LogTrace("Exception stack trace: {trace}", ex.StackTrace);
                throw ex;
            }
            return user;
        }

        /// <summary>
        /// Get the user by Id
        /// </summary>
        /// <returns></returns>
        public async Task<Users> GetUserByIdAsync(Guid id)
        {
            logger.LogInformation("Getting user by Id: {Id}", id);
            try
            {
                var result = await db.Users.Where(ent => ent.UserId == id).
                        Select(ent => new { user = ent, ent.UsersPhones }).
                        FirstOrDefaultAsync();
                if (result == null)
                {
                    return null;
                }
                return result.user;
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in getting user by  Id: {Id}", id);
                logger.LogDebug("Exception message: {message}", ex.Message);
                logger.LogTrace("Exception stack trace: {trace}", ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Log in the user
        /// </summary>
        /// <returns></returns>
        public async Task<Users> UserLoggedIn(Guid userId)
        {
            Users user = await GetUserByIdAsync(userId);

            if (user == null)
            {
                logger.LogWarning("Could not find the user with Id: {Id}", userId);
                return null;
            }

            logger.LogInformation("Updating the last login date of the user");
            user.LastLoginOn = DateTime.Now;

            try
            {
                logger.LogInformation("Saving db change after changing the user's last login date");
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Exception in updating the user");
                logger.LogDebug("Exception message: {message}", ex.Message);
                logger.LogTrace("Exception stack trace: {trace}", ex.StackTrace);
                throw ex ;
            }

            return user;
        }
    }
}
