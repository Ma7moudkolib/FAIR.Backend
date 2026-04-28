using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class UserRepository : RepositoryBase<AppUser>, IUserRepository
    {
        protected readonly UserManager<AppUser> _userManager;

        public UserRepository(dbContext context, UserManager<AppUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<bool> CreateUserAsync(AppUser user)
        {
            return (await _userManager.CreateAsync(user, user.PasswordHash!)).Succeeded;
        }

        public async Task<bool> ChechPasswordAsync(AppUser user, string password)
        {
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
         => await FindByCondition(u => u.UserName == username, false).FirstOrDefaultAsync();

        public async Task<AppUser> GetByEmailAsync(string email)
        {
           return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Player> GetPlayerByIdAsync(string id, bool trackChanges)
        => await FindByCondition(u => u.Id == id, trackChanges).OfType<Player>().FirstOrDefaultAsync();

        public async Task<Coach> GetCoachByIdAsync(string id, bool trackChanges)
        => await FindByCondition(u => u.Id == id, trackChanges)
        .OfType<Coach>().FirstOrDefaultAsync();

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string CurrentPassword, string NewPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.ChangePasswordAsync(user!, CurrentPassword, NewPassword);
        }
        public async Task<AppUser> GetByIdAsync(string id, bool trackChanges)
        => await FindByCondition(u => u.Id == id, trackChanges).FirstOrDefaultAsync();

        public async Task<List<AppUser>> GetUsersByIdsAsync(IEnumerable<string> ids)
         => await FindByCondition(u => ids.Contains(u.Id), false).ToListAsync();
    }
}
