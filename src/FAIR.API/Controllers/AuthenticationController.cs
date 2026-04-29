using FAIR.Application.DTOs.Identity;
using FAIR.Application.Services.Interfaces.Managers;
using Microsoft.AspNetCore.Mvc;

namespace FAIR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateUser(Register user)
        {
            var result = await serviceManager.AuthenticationService.CreateUser(user);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(Login user)
        {
            var result = await serviceManager.AuthenticationService.LoginUser(user);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("refreshToken/{refreshToken}")]
        public async Task<IActionResult> ReviveToken(string refreshToken)
        {
            var result = await serviceManager.AuthenticationService.RevivToken(refreshToken);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
