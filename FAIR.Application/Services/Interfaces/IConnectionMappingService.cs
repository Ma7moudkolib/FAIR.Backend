namespace FAIR.Application.Services.Interfaces
{
    public interface IConnectionMappingService
    {
        void AddOrUpdate(string userId, string connectionId);
        void Remove(string connectionId);
        string? GetConnectionId(string userId);
        IReadOnlyCollection<string> GetConnections(string userId);
    }
}
