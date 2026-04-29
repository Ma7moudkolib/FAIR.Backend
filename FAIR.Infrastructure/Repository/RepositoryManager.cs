using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Infrastructure.Context;
using Microsoft.Extensions.Configuration;

namespace FAIR.Infrastructure.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly dbContext _context;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IAthleteRepository> _athleteRepository;
        private readonly Lazy<ICoachRepository> _coachRepository;

        private readonly Lazy<IVideoAnalysisRepository> _videoAnalysisRepository;
        private readonly Lazy<IChatRepository> _chatRepository;
        private readonly Lazy<ITokenManagement> _tokenManagement;

        public RepositoryManager(dbContext context, IConfiguration configuration)
        {
            _context = context;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context));
            _athleteRepository = new Lazy<IAthleteRepository>(() => new AthleteRepository(_context));
            _coachRepository = new Lazy<ICoachRepository>(() => new CoachRepository(_context));

            _videoAnalysisRepository = new Lazy<IVideoAnalysisRepository>(() => new VideoAnalysisRepository(_context));
            _chatRepository = new Lazy<IChatRepository>(() => new ChatRepository(_context));
            _tokenManagement = new Lazy<ITokenManagement>(() => new TokenManagement(_context, configuration));
        }

        public IUserRepository UserRepository => _userRepository.Value;
        public IAthleteRepository AthleteRepository => _athleteRepository.Value;
        public ICoachRepository CoachRepository => _coachRepository.Value;

        public IVideoAnalysisRepository VideoAnalysis => _videoAnalysisRepository.Value;
        public IChatRepository ChatRepository => _chatRepository.Value;
        public ITokenManagement TokenManagement => _tokenManagement.Value;

        public Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
