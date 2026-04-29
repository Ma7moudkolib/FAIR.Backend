using FAIR.Domain.Entities.Identity;

namespace FAIR.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<AppUser?> GetAnyByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<AppUser?> GetAnyByIdAsync(string id, CancellationToken cancellationToken = default);
    }
}
