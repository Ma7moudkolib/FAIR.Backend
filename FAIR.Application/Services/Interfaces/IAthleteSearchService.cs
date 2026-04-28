using FAIR.Application.DTOs.Search;

namespace FAIR.Application.Services.Interfaces
{
    public interface IAthleteSearchService
    {
        Task<IReadOnlyList<AthleteSearchResult>> SearchAsync(AthleteSearchFilter filter, CancellationToken cancellationToken = default);
    }
}
