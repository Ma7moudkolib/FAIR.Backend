using AutoMapper;

using FAIR.Application.DTOs.Identity;

using FAIR.Application.Services.Implementations;

using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Application.Validations;
using FAIR.Domain.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Application.Services.Managers
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IRepositoryManager> _repositoryManager;
        private readonly Lazy<IAuthenticationService> _authenticationService;
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<UserManager<AppUser>> _userManager;
        private readonly Lazy<IVideoService> _videoService;
        private readonly Lazy<IAthleteSearchService> _athleteSearchService;

        private readonly Lazy<IAiVideoService> _aiVideoService;
        private readonly Lazy<IMapper> _mapper;
        private readonly Lazy<IConnectionMappingService> _connectionMappingService;

        public ServiceManager(IServiceProvider serviceProvider)
        {
            _repositoryManager = new Lazy<IRepositoryManager>(() => serviceProvider.GetRequiredService<IRepositoryManager>());
            _mapper = new Lazy<IMapper>(() => serviceProvider.GetRequiredService<IMapper>());
            _userManager = new Lazy<UserManager<AppUser>>(() => serviceProvider.GetRequiredService<UserManager<AppUser>>());
            _aiVideoService = new Lazy<IAiVideoService>(() => serviceProvider.GetRequiredService<IAiVideoService>());

            _authenticationService = new Lazy<IAuthenticationService>(() => new AuthenticationService(
                _repositoryManager.Value,
                _mapper.Value,
                serviceProvider.GetRequiredService<IValidator<Register>>(),
                serviceProvider.GetRequiredService<IValidator<Login>>(),
                serviceProvider.GetRequiredService<IValidationService>()));

            _userService = new Lazy<IUserService>(() => new UserService(_repositoryManager.Value, _userManager.Value, _mapper.Value));


            _videoService = new Lazy<IVideoService>(() => new VideoService(_repositoryManager.Value, _aiVideoService.Value, _mapper.Value));

            _athleteSearchService = new Lazy<IAthleteSearchService>(() => new AthleteSearchService(_repositoryManager.Value, _mapper.Value));

            _connectionMappingService = new Lazy<IConnectionMappingService>(() => new ConnectionMappingService());
        }

        public IAuthenticationService AuthenticationService => _authenticationService.Value;
        public IUserService UserService => _userService.Value;

        public IVideoService VideoService => _videoService.Value;
        public IAthleteSearchService AthleteSearchService => _athleteSearchService.Value;
        public IConnectionMappingService ConnectionMappingService => _connectionMappingService.Value;

    }
}
