using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Profile;

namespace FAIR.Application.Services.Interfaces
{
    public interface ICoachService
    {
        Task<CoachProfile> GetCoachProfileAsync(string coachId);
        Task<ServiceResponse> UpdateCoachProfileAsync(UpdateCoachProfile profile);
        Task<ServiceResponse> ChangePasswordAsync(string coachId, ChangePasswordRequest request);
    }
}
