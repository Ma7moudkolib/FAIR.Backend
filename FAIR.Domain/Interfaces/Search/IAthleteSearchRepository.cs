using FAIR.Domain.Entities.Identity;

namespace FAIR.Domain.Interfaces.Search
{
    public interface IAthleteSearchRepository
    {
        IQueryable<Player> QueryAthletes();
    }
}
