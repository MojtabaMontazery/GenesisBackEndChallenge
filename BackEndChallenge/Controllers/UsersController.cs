using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DALCore.DB;
using BLLCore.Repositories;
using Newtonsoft.Json.Linq;
using System.Net;
using Swashbuckle.AspNetCore.SwaggerGen;
using BackEndChallenge.Utilities;
using Microsoft.Extensions.Logging;

namespace BackEndChallenge.Controllers
{
    /// <summary>
    /// End points to controll users.
    /// </summary>
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        private readonly IUserRepository repository;
        private readonly ILogger logger;
        private const string dateFormat = "dd/MM/yyyy HH:mm:ss";

        public UsersController(IUserRepository repository, ILogger<UsersController> logger=null)
        {
            this.repository = repository;

            ILoggerFactory loggerFactory = new LoggerFactory()
                        .AddConsole()
                        .AddDebug();
            logger = loggerFactory.CreateLogger<UsersController>();

            this.repository.Logger = logger;
            this.logger = logger;
        }


        /// <summary>
        /// The method creates a new user
        /// </summary>
        /// <param name="fields">A json object in the form of {
        ///"name": "string",
        /// "email": "email@website.com",
        /// "password": "password",
        /// "telephones": [ { "number": "123456789" } , { "number": "877798924" } ]
        ///}</param>
        /// <returns>A json object representing the created user in the form of 
        ///{
        /// "name": "string",
        /// "email": "email@website.com",
        /// "telephones": [ { "number": "123456789" }, { "number": "877798924" }  ],
        /// "id": "GUID"
        ///"createdOn": "dd/MM/yyyy HH:mm:ss",
        /// lastUpdatedOn: "dd/MM/yyyy HH:mm:ss",
        ///lastLoginOn: "dd/MM/yyyy HH:mm:ss",
        ///token: "JWT token"
        ///}
        /// or an error in the form of
        /// {
        /// "message": "error"
        /// }
        /// </returns>
        // POST: api/users/signup
        [Route("SignUp")]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created)]
        public async Task<IActionResult> SignUp([FromBody]JObject fields)
        {
            logger.LogInformation("Parsing input parameters");
            if (fields == null) return BadRequest(new { message = "The required parameters are not passed" });

            string email = fields["email"]?.ToString();
            if (string.IsNullOrWhiteSpace(email)) return BadRequest(new { message = "The parameter email is missed." });

            //running a task async to check if the user already exists, in the meantime we proceed to parse the rest of parameters
            var checkingUserAlreadyExists = repository.UserAlreadyExists(email);

            #region Fetching Other Fields
            string name = fields["name"]?.ToString();
            if (string.IsNullOrWhiteSpace(name)) return BadRequest(new { message = "The parameter name is missed." });


            string password = fields["password"]?.ToString();
            if (string.IsNullOrWhiteSpace(password)) return BadRequest(new { message = "The parameter password is missed." });

            JObject[] telephones = fields["telephones"]?.ToObject<JObject[]>();
            if (telephones == null) return BadRequest(new { message = "The parameter telephones is missed." });

            //reterive phone numbers from the json object
            string[] numbers = telephones.Select(ent => ent["number"]?.ToString()).Where(ent => !string.IsNullOrWhiteSpace(ent)).ToArray();
            if (numbers.Length != telephones.Length) return BadRequest(new { message = "The parameter telephones is not well-formatted." });

            #endregion

            logger.LogInformation("Generating token for the user");
            string token = JwtManager.GenerateToken(name);

            try
            {
                await Task.WhenAll(checkingUserAlreadyExists);
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "There has been an internal error. Please try later." });
            }

            if (checkingUserAlreadyExists.Result)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { message = "E-mail already exists." });
            }

            DALCore.DB.Users dbUser;
            try
            {
                dbUser = await repository.CreateUser(name, email, password, token, numbers);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "There has been an internal error. Please try later." });
            }

            return CreatedAtAction("SearchForUser", new { id = dbUser.UserId }, new
            {
                name = dbUser.Name,
                email = dbUser.Email,
                telephones = dbUser.UsersPhones.Select(ent => new { number = ent.Phone }).ToArray(),
                id = dbUser.UserId,
                createdOn = dbUser.CreatedOn.ToString(dateFormat),
                lastUpdatedOn = dbUser.LastUpdatedOn?.ToString(dateFormat),
                lastLoginOn = dbUser.LastLoginOn.ToString(dateFormat),
                token = dbUser.Token,
            });
        }

        /// <summary>
        /// The user signs in with this method.
        /// </summary>
        /// <param name="fields">A json object in the form of
        /// {
        /// "email": "string",
        ///"password": "password"
        /// }
        /// </param>
        /// <returns>A json object representing some properties of the user in the form of
        ///{
        /// "id": "GUID"
        ///"createdOn": "dd/MM/yyyy HH:mm:ss",
        /// lastUpdatedOn: "dd/MM/yyyy HH:mm:ss",
        ///lastLoginOn: "dd/MM/yyyy HH:mm:ss",
        ///token: "JWT token"
        ///}
        /// or an error in the form of
        /// {
        /// "message": "error"
        /// }
        /// </returns>
        // POST api/users/signin
        [Route("SignIn")]
        [HttpPut]
        public async Task<IActionResult> SignIn([FromBody]JObject fields)
        {
            logger.LogInformation("Parsing parameters for sign in");

            if (fields == null) return BadRequest(new { message = "The required parameters are not passed" });

            string email = fields["email"]?.ToString();
            if (string.IsNullOrWhiteSpace(email)) return BadRequest(new { message = "The parameter email is missed." });

            string password = fields["password"]?.ToString();
            if (string.IsNullOrWhiteSpace(password)) return BadRequest(new { message = "The parameter password is missed." });

            DALCore.DB.Users dbUser;
            try
            {
                dbUser = await repository.SignInUserAsync(email, password);
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "There has been an internal error. Please try again later." });
            }

            if (dbUser == null)//either user not found or password incorrect
                return NotFound(new { message = "Invalid user and / or password." });


            return Ok(new
            {
                id = dbUser.UserId,
                createdOn = dbUser.CreatedOn.ToString(dateFormat),
                lastUpdatedOn = dbUser.LastUpdatedOn?.ToString(dateFormat),
                lastLoginOn = dbUser.LastLoginOn.ToString(dateFormat),
                token = dbUser.Token,
            });
        }

        /// <summary>
        /// The method return the user by the Id. 
        /// </summary>
        /// <param name="id">The id of user (GUID)</param>
        /// <param name="authorization">The user's token should be passed in the header following the term Bearer.</param>
        /// <returns>A json object representing the found user in the form of 
        ///{
        /// "name": "string",
        /// "email": "string",
        /// "telephones": [ { "number": "123456789" } ],
        /// "id": "GUID"
        ///"createdOn": "dd/MM/yyyy HH:mm:ss",
        /// lastUpdatedOn: "dd/MM/yyyy HH:mm:ss",
        ///lastLoginOn: "dd/MM/yyyy HH:mm:ss",
        ///}
        /// or an error in the form of
        /// {
        /// "message": "error"
        /// }
        /// </returns>
        // GET api/users/5
        [HttpGet("{id}")]
        [JwtAuthentication]// returns unauthorized status code if the token is not passed or set improperly
        public async Task<IActionResult> SearchForUser([FromRoute] Guid id, [FromHeader] string authorization)
        {
            logger.LogInformation("Fetching token from the header");
            string token = authorization.Substring("Bearer ".Length).Trim();

            DALCore.DB.Users dbUser;
            try
            {
                dbUser = await repository.GetUserByIdAsync(id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "There has been an internal error. Please try later." });
            }

            if (dbUser == null)
            {
                return NotFound(new { message = "There is no such a user." });
            }

            if (!dbUser.Token.Equals(token))
            {
                logger.LogWarning("The input token differes from the user's token");
                logger.LogTrace("The user's token: {user} \n The input token: {token}", dbUser.Token, token);//passing sensitive data only in trace
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Unauthorized" });
            }

            if (dbUser.LastLoginOn < DateTime.Now.AddMinutes(-30))
            {
                logger.LogInformation("Session time out");
                return StatusCode(440/*Microsoft's IIS code for Login Time-out*/, new { message = "Invalid Session" });
            }

            return Json(new
            {
                name = dbUser.Name,
                email = dbUser.Email,
                telephones = dbUser.UsersPhones.Select(ent => new { number = ent.Phone }).ToArray(),
                id = dbUser.UserId,
                createdOn = dbUser.CreatedOn.ToString(dateFormat),
                lastUpdatedOn = dbUser.LastUpdatedOn?.ToString(dateFormat),
                lastLoginOn = dbUser.LastLoginOn.ToString(dateFormat),
            });
        }

    }
}