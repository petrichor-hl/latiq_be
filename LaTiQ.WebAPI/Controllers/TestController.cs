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
            return Ok("Congratulation! The user has been authenticated.");
        }

        [AllowAnonymous]
        [HttpGet("timeout")]
        public async Task<ActionResult<string>> TestTimeout()
        {
            await Task.Delay(100000);
            return Ok("100 seconds have passed :)");
        }

        [AllowAnonymous]
        [HttpGet("server-error")]
        public ActionResult<string> TestServerError()
        {
            return StatusCode(500, "An error occurred.");
        }
    }
}
