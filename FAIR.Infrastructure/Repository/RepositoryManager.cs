using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Interfaces;
using FAIR.Domain.Interfaces.Search;
using FAIR.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FAIR.Infrastructure.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly dbContext _context;
        private readonly Lazy<IUserRepository> _userRepository;

        private readonly Lazy<IVideoAnalysisRepository> _videoAnalysisRepository;
        private readonly Lazy<IChatRepository> _chatRepository;
        private readonly Lazy<IAthleteSearchRepository> _athleteSearchRepository;
        private readonly Lazy<ITokenManagement> _tokenManagement;

        public RepositoryManager(dbContext context, UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context, userManager));

            _videoAnalysisRepository = new Lazy<IVideoAnalysisRepository>(() => new VideoAnalysisRepository(_context));
            _chatRepository = new Lazy<IChatRepository>(() => new ChatRepository(_context));
            _athleteSearchRepository = new Lazy<IAthleteSearchRepository>(() => new AthleteSearchRepository(_context));
            _tokenManagement = new Lazy<ITokenManagement>(() => new TokenManagement(_context, configuration));
        }

        public IUserRepository UserRepository => _userRepository.Value;

        public IVideoAnalysisRepository VideoAnalysis => _videoAnalysisRepository.Value;
        public IChatRepository ChatRepository => _chatRepository.Value;
        public IAthleteSearchRepository AthleteSearchRepository => _athleteSearchRepository.Value;
        public ITokenManagement TokenManagement => _tokenManagement.Value;

        public Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
