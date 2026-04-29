using AutoMapper;

using FAIR.Application.DTOs.Identity;

using FAIR.Application.Services.Implementations;

using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Application.DTOs.Profile;
using FAIR.Application.DTOs.Search;
using FAIR.Application.DTOs.Video;
using FAIR.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FAIR.Application.Services.Managers
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IRepositoryManager> _repositoryManager;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IAthleteService> _athleteService;
        private readonly Lazy<ICoachService> _coachService;
        private readonly Lazy<IVideoService> _videoService;
        private readonly Lazy<IChatService> _chatService;

        private readonly Lazy<IAiVideoService> _aiVideoService;
        private readonly Lazy<IMapper> _mapper;
        private readonly Lazy<IConnectionMappingService> _connectionMappingService;

        public ServiceManager(IServiceProvider serviceProvider)
        {
            _repositoryManager = new Lazy<IRepositoryManager>(() => serviceProvider.GetRequiredService<IRepositoryManager>());
            _mapper = new Lazy<IMapper>(() => serviceProvider.GetRequiredService<IMapper>());
            _aiVideoService = new Lazy<IAiVideoService>(() => serviceProvider.GetRequiredService<IAiVideoService>());

            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                _repositoryManager.Value,
                _mapper.Value,
                serviceProvider.GetRequiredService<IValidator<Register>>(),
                serviceProvider.GetRequiredService<IValidator<Login>>()));

            _athleteService = new Lazy<IAthleteService>(() => new AthleteService(
                _repositoryManager.Value,
                _mapper.Value,
                serviceProvider.GetRequiredService<IValidator<UpdateAthleteProfile>>(),
                serviceProvider.GetRequiredService<IValidator<ChangePasswordRequest>>(),
                serviceProvider.GetRequiredService<IValidator<AthleteSearchFilter>>()));
            _coachService = new Lazy<ICoachService>(() => new CoachService(
                _repositoryManager.Value,
                _mapper.Value,
                serviceProvider.GetRequiredService<IValidator<UpdateCoachProfile>>(),
                serviceProvider.GetRequiredService<IValidator<ChangePasswordRequest>>()));

            _videoService = new Lazy<IVideoService>(() => new VideoService(
                _repositoryManager.Value,
                _aiVideoService.Value,
                _mapper.Value,
                serviceProvider.GetRequiredService<IValidator<VideoUploadDto>>()));
            _chatService = new Lazy<IChatService>(() => new ChatService(_repositoryManager.Value));

            _connectionMappingService = new Lazy<IConnectionMappingService>(() => new ConnectionMappingService());
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IAthleteService AthleteService => _athleteService.Value;
        public ICoachService CoachService => _coachService.Value;

        public IVideoService VideoService => _videoService.Value;
        public IChatService ChatService => _chatService.Value;
        public IConnectionMappingService ConnectionMappingService => _connectionMappingService.Value;

    }
}
