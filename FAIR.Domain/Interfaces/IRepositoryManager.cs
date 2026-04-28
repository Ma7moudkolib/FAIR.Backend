using FAIR.Domain.Interfaces.Search;

namespace FAIR.Domain.Interfaces
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }

        IVideoAnalysisRepository VideoAnalysis { get; }
        IChatRepository ChatRepository { get; }
        IAthleteSearchRepository AthleteSearchRepository { get; }
        ITokenManagement TokenManagement { get; }
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
