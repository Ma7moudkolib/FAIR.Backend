using AutoMapper;
using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Identity;
using FAIR.Application.Exceptions;
using FAIR.Application.Services.Interfaces;
using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace FAIR.Application.Services.Implementations
{
    public class AuthenticationService(
        IRepositoryManager repositoryManager,
        IMapper mapper,
        IValidator<Register> registerValidation,
        IValidator<Login> loginUserValidation) : IAuthenticationService
    {
        public async Task<ServiceResponse> CreateUser(Register createUser)
        {
            var validationResult = await registerValidation.ValidateAsync(createUser);
            if (!validationResult.IsValid)
            {
                throw new ServiceValidationException(validationResult.Errors);
            }

            var isAthlete = createUser.Role.ToLower() == "athlete";
            var isCoach = createUser.Role.ToLower() == "coach";

            if (!isAthlete && !isCoach)
            {
                return new ServiceResponse(false, "This Role is not available. Please use 'athlete' or 'coach'.");
            }

            if (await repositoryManager.UserRepository.EmailExistsAsync(createUser.Email))
            {
                return new ServiceResponse(false, "Email is already registered!");
            }

            if (await repositoryManager.UserRepository.UsernameExistsAsync(createUser.Username))
            {
                return new ServiceResponse(false, "UserName is already registered!");
            }


            if (isAthlete)
            {
                var athlete = mapper.Map<Athlete>(createUser);
                athlete.Role = "Athlete";
                athlete.PasswordHash = HashPassword(createUser.Password);
                repositoryManager.AthleteRepository.CreateAthleteAsync(athlete);
            }
            else if (isCoach)
            {
                var coach = mapper.Map<Coach>(createUser);
                coach.Role = "Coach";
                coach.PasswordHash = HashPassword(createUser.Password);
                repositoryManager.CoachRepository.CreateCoachAsync(coach);
            }

           int isCreated = await repositoryManager.SaveAsync();

            return isCreated > 0
                ? new ServiceResponse(true, "Created Account")
                : new ServiceResponse(false, "Error occurred creating the Account");
        }

        public async Task<LoginResponse> LoginUser(Login login)
        {
            var validationResult = await loginUserValidation.ValidateAsync(login);
            if (!validationResult.IsValid)
            {
                throw new ServiceValidationException(validationResult.Errors);
            }

            var user = await repositoryManager.UserRepository.GetAnyByUsernameAsync(login.Username);

            if (user == null)
            {
                return new LoginResponse(message: "Invalid UserName or Password");
            }

            if (!VerifyPassword(login.Password, user.PasswordHash!))
            {
                return new LoginResponse(message: "Invalid UserName or Password");
            }

            var token = repositoryManager.TokenManagement.GenerateToken(user);
            var refreshToken = repositoryManager.TokenManagement.GetRefreshToken();
            var saveTokenResult = await repositoryManager.TokenManagement.AddRefreshToken(user.Id, refreshToken);

            return saveTokenResult <= 0
                ? new LoginResponse(message: "Internal error occurred while authenticating.")
                : new LoginResponse(Success: true, Token: token, refreshToken: refreshToken);
        }

        public async Task<LoginResponse> RevivToken(string refreshToken)
        {
            var validateTokenResult = await repositoryManager.TokenManagement.ValidateRefreshToken(refreshToken);
            if (!validateTokenResult)
            {
                return new LoginResponse(message: "Invalid Token");
            }

            var userId = await repositoryManager.TokenManagement.GetUserIdByRefreshToken(refreshToken);
            
            var user = await repositoryManager.UserRepository.GetAnyByIdAsync(userId);

            if (user == null)
            {
                return new LoginResponse(message: "User not found.");
            }

            var newtoken = repositoryManager.TokenManagement.GenerateToken(user);
            var newrefreshToken = repositoryManager.TokenManagement.GetRefreshToken();
            await repositoryManager.TokenManagement.UpdateRefreshToken(newrefreshToken);
            return new LoginResponse(Success: true, Token: newtoken, refreshToken: refreshToken);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword == storedHash;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
