namespace FAIR.Application.Services.Interfaces.Managers
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IUserService UserService { get; }

        IVideoService VideoService { get; }
        IAthleteSearchService AthleteSearchService { get; }
        IConnectionMappingService ConnectionMappingService { get; }

    }
}
