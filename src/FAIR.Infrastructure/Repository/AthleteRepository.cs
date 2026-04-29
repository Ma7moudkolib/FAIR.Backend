using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class AthleteRepository : UserRepository, IAthleteRepository
    {
        private readonly dbContext _context;

        public AthleteRepository(dbContext context) : base(context)
        {
            _context = context;
        }

        public void CreateAthleteAsync(Athlete athlete) => _context.Athletes.Add(athlete);

        public async Task<Athlete?> GetByUsernameAsync(string username)
        {
            return await _context.Athletes.AsNoTracking().SingleOrDefaultAsync(a => a.UserName == username);
        }

        public async Task<Athlete?> GetByEmailAsync(string email)
        {
            return await _context.Athletes.AsNoTracking().SingleOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Athlete?> GetByIdAsync(string id, bool trackChanges)
        {
            return trackChanges
                ? await _context.Athletes.SingleOrDefaultAsync(a => a.Id == id)
                : await _context.Athletes.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Athlete>> GetAthletesByIdsAsync(IEnumerable<string> ids)
        {
            return await _context.Athletes.AsNoTracking().Where(a => ids.Contains(a.Id)).ToListAsync();
        }

        public IQueryable<Athlete> QueryAthletes() => _context.Athletes.AsNoTracking().AsQueryable();
    }
}
