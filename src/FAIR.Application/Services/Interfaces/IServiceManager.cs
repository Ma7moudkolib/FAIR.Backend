namespace FAIR.Application.Services.Interfaces.Managers
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IAthleteService AthleteService { get; }
        ICoachService CoachService { get; }
        IVideoService VideoService { get; }
        IChatService ChatService { get; }
        IConnectionMappingService ConnectionMappingService { get; }

    }
}
