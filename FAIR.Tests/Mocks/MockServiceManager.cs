using AutoMapper;
using FAIR.Application.Services.Interfaces;
using FAIR.Application.Services.Interfaces.Managers;
using FAIR.Tests.Fixtures;
using Moq;

namespace FAIR.Tests.Mocks
{
    /// <summary>
    /// Factory for creating pre-configured Mock IServiceManager instances.
    /// Provides fluent API for test-specific mock setup.
    /// </summary>
    public class MockServiceManager
    {
        private readonly Mock<IServiceManager> _mock;
        private readonly Mock<IAuthenticationService> _authenticationServiceMock;
        private readonly Mock<IAthleteService> _athleteServiceMock;
        private readonly Mock<ICoachService> _coachServiceMock;
        private readonly Mock<IVideoService> _videoServiceMock;
        private readonly Mock<IChatService> _chatServiceMock;
        private readonly Mock<IConnectionMappingService> _connectionMappingServiceMock;
        private readonly IMapper _mapper;

        public MockServiceManager(MapperFixture mapperFixture)
        {
            _mapper = mapperFixture.Mapper;

            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _athleteServiceMock = new Mock<IAthleteService>();
            _coachServiceMock = new Mock<ICoachService>();
            _videoServiceMock = new Mock<IVideoService>();
            _chatServiceMock = new Mock<IChatService>();
            _connectionMappingServiceMock = new Mock<IConnectionMappingService>();

            _mock = new Mock<IServiceManager>();

            // Setup default properties with mocked services
            _mock.Setup(m => m.AuthenticationService).Returns(_authenticationServiceMock.Object);
            _mock.Setup(m => m.AthleteService).Returns(_athleteServiceMock.Object);
            _mock.Setup(m => m.CoachService).Returns(_coachServiceMock.Object);
            _mock.Setup(m => m.VideoService).Returns(_videoServiceMock.Object);
            _mock.Setup(m => m.ChatService).Returns(_chatServiceMock.Object);
            _mock.Setup(m => m.ConnectionMappingService).Returns(_connectionMappingServiceMock.Object);
        }

        public Mock<IServiceManager> Mock => _mock;

        public IServiceManager Object => _mock.Object;

        public Mock<IAuthenticationService> AuthenticationServiceMock => _authenticationServiceMock;

        public Mock<IAthleteService> AthleteServiceMock => _athleteServiceMock;

        public Mock<ICoachService> CoachServiceMock => _coachServiceMock;

        public Mock<IVideoService> VideoServiceMock => _videoServiceMock;
        public Mock<IChatService> ChatServiceMock => _chatServiceMock;

        public Mock<IConnectionMappingService> ConnectionMappingServiceMock => _connectionMappingServiceMock;

        public IMapper Mapper => _mapper;

        /// <summary>
        /// Resets all service mock call counts.
        /// </summary>
        public void ResetMocks()
        {
            _mock.Reset();
            _authenticationServiceMock.Reset();
            _athleteServiceMock.Reset();
            _coachServiceMock.Reset();
            _videoServiceMock.Reset();
            _chatServiceMock.Reset();
            _connectionMappingServiceMock.Reset();

            // Re-setup after reset
            _mock.Setup(m => m.AuthenticationService).Returns(_authenticationServiceMock.Object);
            _mock.Setup(m => m.AthleteService).Returns(_athleteServiceMock.Object);
            _mock.Setup(m => m.CoachService).Returns(_coachServiceMock.Object);
            _mock.Setup(m => m.VideoService).Returns(_videoServiceMock.Object);
            _mock.Setup(m => m.ChatService).Returns(_chatServiceMock.Object);
            _mock.Setup(m => m.ConnectionMappingService).Returns(_connectionMappingServiceMock.Object);
        }
    }
}
