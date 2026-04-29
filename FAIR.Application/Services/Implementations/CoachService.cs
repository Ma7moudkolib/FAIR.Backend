using AutoMapper;
using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.Exceptions;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Interfaces;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace FAIR.Application.Services.Implementations
{
    public class CoachService(
        IRepositoryManager repositoryManager,
        IMapper mapper,
        IValidator<UpdateCoachProfile> updateCoachProfileValidator,
        IValidator<ChangePasswordRequest> changePasswordRequestValidator) : ICoachService
    {
        public async Task<CoachProfile> GetCoachProfileAsync(string coachId)
        {
            var coach = await repositoryManager.CoachRepository.GetByIdAsync(coachId, false);
            if (coach is null)
                return new CoachProfile();
            return mapper.Map<CoachProfile>(coach);
        }

        public async Task<ServiceResponse> UpdateCoachProfileAsync(UpdateCoachProfile profile)
        {
            var validationResult = await updateCoachProfileValidator.ValidateAsync(profile);
            if (!validationResult.IsValid)
            {
                throw new ServiceValidationException(validationResult.Errors);
            }

            var coach = await repositoryManager.CoachRepository.GetByIdAsync(profile.Id, true);
            if (coach is null)
                return new ServiceResponse(false, "Coach Not Found!");

            mapper.Map(profile, coach);
            await repositoryManager.SaveAsync();
            return new ServiceResponse(true, "Update Profile!");
        }

        public async Task<ServiceResponse> ChangePasswordAsync(string coachId, ChangePasswordRequest request)
        {
            var validationResult = await changePasswordRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ServiceValidationException(validationResult.Errors);
            }

            var coach = await repositoryManager.CoachRepository.GetByIdAsync(coachId, true);
            if (coach is null)
            {
                return new ServiceResponse(false, "Coach Not Found!");
            }

            if (!VerifyPassword(request.CurrentPassword, coach.PasswordHash))
            {
                return new ServiceResponse(false, "Incorrect current password");
            }

            coach.PasswordHash = HashPassword(request.NewPassword);
            await repositoryManager.SaveAsync();

            return new ServiceResponse(true, "Success to Change Password");
        }

        private static bool VerifyPassword(string password, string? storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword == storedHash;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
