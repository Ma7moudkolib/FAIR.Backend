using FAIR.Domain.Interfaces;
using Moq;

namespace FAIR.Tests.Mocks
{
    /// <summary>
    /// Factory for creating pre-configured Mock IRepositoryManager instances.
    /// Provides fluent API for test-specific mock setup.
    /// </summary>
    public class MockRepositoryManager
    {
        private readonly Mock<IRepositoryManager> _mock;
        private readonly Mock<IAthleteRepository> _athleteRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ICoachRepository> _coachRepositoryMock;
        private readonly Mock<IVideoAnalysisRepository> _videoAnalysisRepositoryMock;
        private readonly Mock<IChatRepository> _chatRepositoryMock;
        private readonly Mock<ITokenManagement> _tokenManagementMock;

        public MockRepositoryManager()
        {
            _athleteRepositoryMock = new Mock<IAthleteRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _coachRepositoryMock = new Mock<ICoachRepository>();
            _videoAnalysisRepositoryMock = new Mock<IVideoAnalysisRepository>();
            _chatRepositoryMock = new Mock<IChatRepository>();
            _tokenManagementMock = new Mock<ITokenManagement>();

            _mock = new Mock<IRepositoryManager>();

            // Setup default properties with mocked repositories
            _mock.Setup(m => m.UserRepository).Returns(_userRepositoryMock.Object);
            _mock.Setup(m => m.AthleteRepository).Returns(_athleteRepositoryMock.Object);
            _mock.Setup(m => m.CoachRepository).Returns(_coachRepositoryMock.Object);
            _mock.Setup(m => m.VideoAnalysis).Returns(_videoAnalysisRepositoryMock.Object);
            _mock.Setup(m => m.ChatRepository).Returns(_chatRepositoryMock.Object);
            _mock.Setup(m => m.TokenManagement).Returns(_tokenManagementMock.Object);

            // Default SaveAsync behavior
            _mock.Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        public Mock<IRepositoryManager> Mock => _mock;

        public IRepositoryManager Object => _mock.Object;

        public Mock<IUserRepository> UserRepositoryMock => _userRepositoryMock;

        public Mock<IAthleteRepository> AthleteRepositoryMock => _athleteRepositoryMock;

        public Mock<ICoachRepository> CoachRepositoryMock => _coachRepositoryMock;

        public Mock<IVideoAnalysisRepository> VideoAnalysisRepositoryMock => _videoAnalysisRepositoryMock;

        public Mock<IChatRepository> ChatRepositoryMock => _chatRepositoryMock;

        public Mock<ITokenManagement> TokenManagementMock => _tokenManagementMock;

        /// <summary>
        /// Verifies that SaveAsync was called exactly once.
        /// </summary>
        public void VerifySaveAsyncCalled(Times? times = null)
        {
            var timesValue = times ?? Times.Once();
            _mock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), timesValue);
        }

        /// <summary>
        /// Verifies that SaveAsync was never called.
        /// </summary>
        public void VerifySaveAsyncNotCalled()
        {
            _mock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Resets all mock call counts.
        /// </summary>
        public void ResetMocks()
        {
            _mock.Reset();
            _athleteRepositoryMock.Reset();
            _userRepositoryMock.Reset();
            _coachRepositoryMock.Reset();
            _videoAnalysisRepositoryMock.Reset();
            _chatRepositoryMock.Reset();
            _tokenManagementMock.Reset();

            // Re-setup after reset
            _mock.Setup(m => m.UserRepository).Returns(_userRepositoryMock.Object);
            _mock.Setup(m => m.AthleteRepository).Returns(_athleteRepositoryMock.Object);
            _mock.Setup(m => m.CoachRepository).Returns(_coachRepositoryMock.Object);
            _mock.Setup(m => m.VideoAnalysis).Returns(_videoAnalysisRepositoryMock.Object);
            _mock.Setup(m => m.ChatRepository).Returns(_chatRepositoryMock.Object);
            _mock.Setup(m => m.TokenManagement).Returns(_tokenManagementMock.Object);
            _mock.Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
        }

        /// <summary>
        /// Configures SaveAsync to throw an exception.
        /// </summary>
        public void SetupSaveAsyncToThrow(Exception exception)
        {
            _mock.Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }

        /// <summary>
        /// Configures SaveAsync to return a specific number.
        /// </summary>
        public void SetupSaveAsyncToReturn(int numberOfAffectedRows)
        {
            _mock.Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(numberOfAffectedRows);
        }
    }
}
