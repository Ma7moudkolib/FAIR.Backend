using FAIR.Domain.Entities.Identity;

namespace FAIR.Domain.Interfaces
{
    public interface ICoachRepository
    {
        void CreateCoachAsync(Coach coach);
        Task<Coach?> GetByUsernameAsync(string username);
        Task<Coach?> GetByEmailAsync(string email);
        Task<Coach?> GetByIdAsync(string id, bool trackChanges);
        Task<List<Coach>> GetCoachesByIdsAsync(IEnumerable<string> ids);
    }
}
