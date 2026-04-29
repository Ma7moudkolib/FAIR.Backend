using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class UserRepository(dbContext context) : IUserRepository
    {
        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalized = email.Trim().ToLower();
            return await context.Athletes.AsNoTracking().AnyAsync(a => a.Email != null && a.Email.ToLower() == normalized, cancellationToken)
                || await context.Coaches.AsNoTracking().AnyAsync(c => c.Email != null && c.Email.ToLower() == normalized, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            var normalized = username.Trim().ToLower();
            return await context.Athletes.AsNoTracking().AnyAsync(a => a.UserName != null && a.UserName.ToLower() == normalized, cancellationToken)
                || await context.Coaches.AsNoTracking().AnyAsync(c => c.UserName != null && c.UserName.ToLower() == normalized, cancellationToken);
        }

        public async Task<AppUser?> GetAnyByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var normalized = username.Trim().ToLower();
            AppUser? user = await context.Athletes.AsNoTracking()
                .FirstOrDefaultAsync(a => a.UserName != null && a.UserName.ToLower() == normalized, cancellationToken);
            user ??= await context.Coaches.AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserName != null && c.UserName.ToLower() == normalized, cancellationToken);
            return user;
        }

        public async Task<AppUser?> GetAnyByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            AppUser? user = await context.Athletes.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            user ??= await context.Coaches.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            return user;
        }
    }
}
