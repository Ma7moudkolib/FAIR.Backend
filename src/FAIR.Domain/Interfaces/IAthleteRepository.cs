using FAIR.Domain.Entities.Identity;

namespace FAIR.Domain.Interfaces
{
    public interface IAthleteRepository
    {
        void CreateAthleteAsync(Athlete athlete);
        Task<Athlete?> GetByUsernameAsync(string username);
        Task<Athlete?> GetByEmailAsync(string email);
        Task<Athlete?> GetByIdAsync(string id, bool trackChanges);
        Task<List<Athlete>> GetAthletesByIdsAsync(IEnumerable<string> ids);
        IQueryable<Athlete> QueryAthletes();
    }
}
