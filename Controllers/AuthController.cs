using Auth_web_2.Dtos;
using Auth_web_2.Responses;
using Auth_web_2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth_web_2.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService service;

        public AuthController(AuthService service)
        {
            this.service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDto user)
        {
            var result = await service.RegisterUser(user);

            return result.Status switch
            {
                AuthStatus.AlreadyExists => Conflict(new { message = "Username already exists" }),
                AuthStatus.NotFound => Unauthorized(new { message = "Username not found" }),
                AuthStatus.Success => Ok(new { message = "User Successfully registered" }),
                _ => BadRequest()
            };

        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUsre(UserDto user)
        {
            var result = await service.LoginUser(user);

            return result.Status switch
            {
                AuthStatus.InvalidCredentails => Unauthorized(new { message = "Invalid credentials" }),
                AuthStatus.NotFound => Unauthorized(new { message = "Username not found" }),
                AuthStatus.Success => Ok(new { message = "User Successfully LoggedIn", token = result.Token }),
                _ => BadRequest()
            };
        }

        [HttpGet("check")]
        [Authorize(Roles = "User")]
        public string Check()
        {
            return "Hello User";
        }


        [HttpGet("checkAdmin")]
        [Authorize(Roles = "Admin")]
        public string CheckAdmin()
        {
            return "Hello Admin";
        }
    }
}
