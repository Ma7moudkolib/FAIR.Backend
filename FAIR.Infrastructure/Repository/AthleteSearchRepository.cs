using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces.Search;
using FAIR.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FAIR.Infrastructure.Repository
{
    public class AthleteSearchRepository(dbContext context) : IAthleteSearchRepository
    {
        public IQueryable<Player> QueryAthletes()
        {
            return context.Players.AsNoTracking().AsQueryable();
        }
    }
}
