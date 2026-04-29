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
        [HttpGet("athleteProfile/{athleteId}")]
        public async Task<IActionResult> GetAthleteProfile(string athleteId)
        {
            var profile = await serviceManager.AthleteService.GetAthleteProfileAsync(athleteId);
            return profile != null ? Ok(profile) : NotFound();
        }

        [HttpGet("coachProfile/{coachId}")]
        public async Task<IActionResult> CoachProfile(string coachId)
        {
            var profile = await serviceManager.CoachService.GetCoachProfileAsync(coachId);
            return profile != null ? Ok(profile) : NotFound();
        }

        [HttpPut("updateAthleteProfile")]
        public async Task<IActionResult> UpdateAthleteProfile([FromBody] UpdateAthleteProfile athleteProfile)
        {
            var result = await serviceManager.AthleteService.UpdateAthleteProfileAsync(athleteProfile);
            return Ok(result);
        }

        [HttpPut("updateCoachProfile")]
        public async Task<IActionResult> UpdateCoachProfile([FromBody] UpdateCoachProfile coachProfile)
        {
            var result = await serviceManager.CoachService.UpdateCoachProfileAsync(coachProfile);
            return Ok(result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordRequest request)
        {
            var result = await serviceManager.AthleteService.ChangePasswordAsync(userId, request);
            if (!result.Success && result.message == "Athlete Not Found!")
            {
                result = await serviceManager.CoachService.ChangePasswordAsync(userId, request);
            }
            return Ok(result);
        }
    }
}
