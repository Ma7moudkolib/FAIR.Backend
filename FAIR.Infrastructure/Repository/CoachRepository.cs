using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class CoachRepository : UserRepository, ICoachRepository
    {
        private readonly dbContext _context;

        public CoachRepository(dbContext context) : base(context)
        {
            _context = context;
        }

        public void CreateCoachAsync(Coach coach) => _context.Coaches.Add(coach);

        public async Task<Coach?> GetByUsernameAsync(string username)
        {
            return await _context.Coaches.AsNoTracking().SingleOrDefaultAsync(c => c.UserName == username);
        }

        public async Task<Coach?> GetByEmailAsync(string email)
        {
            return await _context.Coaches.AsNoTracking().SingleOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Coach?> GetByIdAsync(string id, bool trackChanges)
        {
            return trackChanges
                ? await _context.Coaches.SingleOrDefaultAsync(c => c.Id == id)
                : await _context.Coaches.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Coach>> GetCoachesByIdsAsync(IEnumerable<string> ids)
        {
            return await _context.Coaches.AsNoTracking().Where(c => ids.Contains(c.Id)).ToListAsync();
        }
    }
}
