using AutoMapper;
using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Application.Services.Implementations
{
    public class UserService(IRepositoryManager repositoryManager,
     UserManager<AppUser> userManager, IMapper mapper) : IUserService
    {
        public async Task<CoachProfile> GetCoachProfileAsync(string coachId)
        {
            var coach = await repositoryManager.UserRepository.GetCoachByIdAsync(coachId, false);
            if (coach is null)
                return new CoachProfile();
            return mapper.Map<CoachProfile>(coach);
        }

        public async Task<PlayerProfile> GetPlayerProfileAsync(string playerId)
        {
            var player = await repositoryManager.UserRepository.GetPlayerByIdAsync(playerId, false);
            if (player is null)
                return new PlayerProfile();
            return mapper.Map<PlayerProfile>(player);
        }

        public async Task<ServiceResponse> UpdateCoachProfileAsync(UpdateCoachProfile profile)
        {
            var coach = await repositoryManager.UserRepository.GetCoachByIdAsync(profile.Id, true);
            if (coach is null)
                return new ServiceResponse(false, "Coach Not Found!");
            
            mapper.Map(profile, coach);
            await repositoryManager.SaveAsync();
            return new ServiceResponse(true, "Update Profile!");
  
        }

        public async Task<ServiceResponse> UpdatePlayerProfileAsync(UpdatePlayerProfile profile)
        {
            var player = await repositoryManager.UserRepository.GetPlayerByIdAsync(profile.Id, true);
            if (player is null)
                return new ServiceResponse(false, "Player Not Found!");
            mapper.Map(profile, player);
            await repositoryManager.SaveAsync();
            return new ServiceResponse(true, "Update Profile!");
        }

        public async Task<ServiceResponse> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var userEntity = await repositoryManager.UserRepository.GetByIdAsync(userId, false);
            if (userEntity is null)
            {
                return new ServiceResponse(false, "User Not Found!");
            }
            
            var result = await userManager.ChangePasswordAsync(userEntity!, request.CurrentPassword, request.NewPassword);
            
            return result.Succeeded ? new ServiceResponse(true, "Success to Change Password") 
            : new ServiceResponse(false, result.Errors.First().Description);
        }
    }
}
