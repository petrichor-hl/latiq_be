using LaTiQ.Core.DTO.Request;
using LaTiQ.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaTiQ.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("anonymous")]
        public ActionResult<string> TestAnonymous()
        {
            return Ok("Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
        }

        [HttpGet("authenticated")]
        public ActionResult<string> TestAuthenticated()
        {
            ClaimsPrincipal abcxyz = HttpContext.User;
            Console.WriteLine("abcxyz.FindFirstValue(ClaimTypes.Email = " + abcxyz.FindFirstValue(ClaimTypes.Email));
            return Ok("Congratulation! The user has been authenticated.");
        }
    }
}
