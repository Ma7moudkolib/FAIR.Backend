using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.DTOs.Search;

namespace FAIR.Application.Services.Interfaces
{
    public interface IAthleteService
    {
        Task<AthleteProfile> GetAthleteProfileAsync(string athleteId);
        Task<ServiceResponse> UpdateAthleteProfileAsync(UpdateAthleteProfile profile);
        Task<ServiceResponse> ChangePasswordAsync(string athleteId, ChangePasswordRequest request);
        Task<IReadOnlyList<AthleteSearchResult>> SearchAsync(AthleteSearchFilter filter, CancellationToken cancellationToken = default);
    }
}
