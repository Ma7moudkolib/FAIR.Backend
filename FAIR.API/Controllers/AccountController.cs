using FAIR.Application.DTOs.Profile;
using FAIR.Application.Services.Interfaces.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FAIR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpGet("playerProfile/{playerId}")]
        public async Task<IActionResult> GetPlayerProfile(string playerId)
        {
            var profile = await serviceManager.UserService.GetPlayerProfileAsync(playerId);
            return profile != null ? Ok(profile) : NotFound();
        }

        [HttpGet("coachProfile/{coachId}")]
        public async Task<IActionResult> CoachProfile(string coachId)
        {
            var profile = await serviceManager.UserService.GetCoachProfileAsync(coachId);
            return profile != null ? Ok(profile) : NotFound();
        }

        [HttpPut("updatePlayerProfile")]
        public async Task<IActionResult> UpdatePlayerProfile([FromBody] UpdatePlayerProfile playerProfile)
        {
            var result = await serviceManager.UserService.UpdatePlayerProfileAsync(playerProfile);
            return Ok(result);
        }

        [HttpPut("updateCoachProfile")]
        public async Task<IActionResult> UpdateCoachProfile([FromBody] UpdateCoachProfile coachProfile)
        {
            var result = await serviceManager.UserService.UpdateCoachProfileAsync(coachProfile);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordRequest request)
        {
            var result = await serviceManager.UserService.ChangePasswordAsync(userId, request);
            return Ok(result);
        }
    }
}
