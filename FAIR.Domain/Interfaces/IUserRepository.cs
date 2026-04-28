using FAIR.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FAIR.Domain.Interfaces
{
    public interface IUserRepository 
    {
        Task<bool> CreateUserAsync(AppUser user);
        Task<bool> ChechPasswordAsync(AppUser user, string password);
        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByIdAsync(string id, bool trackChanges);
        Task<Player> GetPlayerByIdAsync(string id, bool trackChanges);
        Task<Coach> GetCoachByIdAsync(string id, bool trackChanges);
        Task<IdentityResult> ChangePasswordAsync(string userId, string CurrentPassword, string NewPassword);
        Task<List<AppUser>> GetUsersByIdsAsync(IEnumerable<string> ids);
    }
}
