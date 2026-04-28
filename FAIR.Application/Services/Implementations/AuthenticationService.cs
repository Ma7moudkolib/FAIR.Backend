using AutoMapper;
using FAIR.Application.DTOs;
using FAIR.Application.DTOs.Identity;
using FAIR.Application.Services.Interfaces;
using FAIR.Application.Validations;
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
        IValidator<Login> loginUserValidation,
        IValidationService validation) : IAuthenticationService
    {
        public async Task<ServiceResponse> CreateUser(Register createUser)
        {
            var validationResult = await validation.ValidateAsync(createUser, registerValidation);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            var checkUserEmail = await repositoryManager.UserRepository.GetByEmailAsync(createUser.Email);
            if (checkUserEmail != null)
            {
                return new ServiceResponse(false, "Email is already registered!");
            }

            var checkUserName = await repositoryManager.UserRepository.GetByUsernameAsync(createUser.Username);
            if (checkUserName != null)
            {
                return new ServiceResponse(false, "UserName is already registered!");
            }

            AppUser user;
            if (createUser.Role.ToLower() == "player")
            {
                user = mapper.Map<Player>(createUser);
            }
            else if (createUser.Role.ToLower() == "coach")
            {
                user = mapper.Map<Coach>(createUser);
            }
            else
            {
                return new ServiceResponse(false, "This Role not available.");
            }

            user.PasswordHash = createUser.Password;
            var isCreated = await repositoryManager.UserRepository.CreateUserAsync(user);
            return isCreated
                ? new ServiceResponse(true, "Created Account")
                : new ServiceResponse(false, "Error occure Create the Account");
        }

        public async Task<LoginResponse> LoginUser(Login login)
        {
            var validationResult = await validation.ValidateAsync(login, loginUserValidation);
            if (!validationResult.Success)
            {
                return new LoginResponse(message: validationResult.message);
            }

            var user = await repositoryManager.UserRepository.GetByUsernameAsync(login.Username);
            if (user == null)
            {
                return new LoginResponse(message: "Invalid UserName or Password");
            }

            var isValidPassword = await repositoryManager.UserRepository.ChechPasswordAsync(user, login.Password);
            if (!isValidPassword)
            {
                return new LoginResponse(message: "Invalid UserName or Password");
            }

            var token = repositoryManager.TokenManagement.GenerateToken(user);
            var refreshToken = repositoryManager.TokenManagement.GetRefreshToken();
            var saveTokenResult = await repositoryManager.TokenManagement.AddRefreshToken(user.Id, refreshToken);

            return saveTokenResult <= 0
                ? new LoginResponse(message: "Internal error occurred while authentiacatint.")
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
            var user = await repositoryManager.UserRepository.GetByIdAsync(userId,false);
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
