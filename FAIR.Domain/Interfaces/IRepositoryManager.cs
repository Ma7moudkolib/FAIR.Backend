namespace FAIR.Domain.Interfaces
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        IAthleteRepository AthleteRepository { get; }
        ICoachRepository CoachRepository { get; }

        IVideoAnalysisRepository VideoAnalysis { get; }
        IChatRepository ChatRepository { get; }
        ITokenManagement TokenManagement { get; }
        Task<int> SaveAsync(CancellationToken cancellationToken = default);
    }
}
