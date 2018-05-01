using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEndChallenge.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]//to exclude from Swagger UI
    [Produces("application/json")]
    public class HomeController : Controller
    {
        /// <summary>
        /// This end point is designed to catch all not found end points.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [Route("{*url}")]
        [HttpGet]
        public IActionResult Index(string url)
        {
            return NotFound(new { message = "The resource is not found." });
        }
    }
}