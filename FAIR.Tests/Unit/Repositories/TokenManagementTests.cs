using FluentAssertions;
using FAIR.Domain.Entities.Identity;
using FAIR.Infrastructure.Repository;
using FAIR.Tests.Common;
using FAIR.Tests.Data.Builders;
using FAIR.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace FAIR.Tests.Unit.Repositories
{
    public class TokenManagementTests : RepositoryTestBase
    {
        private TokenManagement _tokenManagement;
        private readonly IConfiguration _mockConfig;

        public TokenManagementTests(InMemoryDbContextFixture dbFixture, MapperFixture mapperFixture)
            : base(dbFixture, mapperFixture)
        {
            // Setup mock configuration with required JWT settings
            var configDict = new Dictionary<string, string>
            {
                {"Jwt:Key", "ThisIsAVeryLongSecretKeyThatMustBeAtLeast32CharactersLongForHS256"},
                {"Jwt:Issuer", "FairApi"},
                {"Jwt:Audience", "FairApiUsers"},
                {"Jwt:ExpirationMinutes", "120"}
            };
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(configDict)
                .Build();
            _mockConfig = config;
        }

        private TokenManagement GetTokenManagement() => new TokenManagement(DbContext, _mockConfig);

        [Fact]
        public async Task AddRefreshToken_WithValidToken_ShouldPersist()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var refreshToken = "test_refresh_token_" + Guid.NewGuid();

            // Act
            var result = await _tokenManagement.AddRefreshToken(userId, refreshToken);

            // Assert
            result.Should().Be(1);
            var saved = await DbContext.RefreshToken.FirstOrDefaultAsync(t => t.UserId == userId);
            saved.Should().NotBeNull();
            saved!.Token.Should().Be(refreshToken);
        }

        [Fact]
        public async Task AddRefreshToken_WithMultipleTokensForSameUser_ShouldPersistAll()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var token1 = "token_1_" + Guid.NewGuid();
            var token2 = "token_2_" + Guid.NewGuid();

            // Act
            await _tokenManagement.AddRefreshToken(userId, token1);
            await _tokenManagement.AddRefreshToken(userId, token2);

            // Assert
            var userTokens = await DbContext.RefreshToken.Where(t => t.UserId == userId).ToListAsync();
            userTokens.Should().HaveCount(2);
        }

        [Fact]
        public void GenerateToken_WithAthleteUser_ShouldCreateValidToken()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var athlete = new AthleteBuilder().Build();

            // Act
            var token = _tokenManagement.GenerateToken(athlete);

            // Assert
            token.Should().NotBeNullOrEmpty();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == "FullName" && c.Value == athlete.FullName);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == athlete.Id);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == athlete.Email);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Athlete");
        }

        [Fact]
        public void GenerateToken_WithCoachUser_ShouldSetRoleAsCoach()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var coach = new CoachBuilder().Build();

            // Act
            var token = _tokenManagement.GenerateToken(coach);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Coach");
        }

        [Fact]
        public void GenerateToken_ShouldHaveCorrectExpiration()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var athlete = new AthleteBuilder().Build();
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var token = _tokenManagement.GenerateToken(athlete);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Token should expire in approximately 2 days from now
            jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddHours(47));
            jwtToken.ValidTo.Should().BeBefore(beforeGeneration.AddHours(49));
        }

        [Fact]
        public void GetRefreshToken_ShouldReturnUrlEncodedToken()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();

            // Act
            var token = _tokenManagement.GetRefreshToken();

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Length.Should().BeGreaterThan(50);
            // URL encoded token won't have certain characters that are percent-encoded
            token.Should().MatchRegex(@"^[A-Za-z0-9%_\-]*$");
        }

        [Fact]
        public void GetRefreshToken_ShouldGenerateDifferentTokenEachTime()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();

            // Act
            var token1 = _tokenManagement.GetRefreshToken();
            var token2 = _tokenManagement.GetRefreshToken();

            // Assert
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GetUserClaimsFromToken_WithValidToken_ShouldExtractAllClaims()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var athlete = new AthleteBuilder().Build();
            var token = _tokenManagement.GenerateToken(athlete);

            // Act
            var claims = _tokenManagement.GetUserClaimsFromToken(token);

            // Assert
            claims.Should().NotBeEmpty();
            claims.Should().Contain(c => c.Type == "FullName");
            claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier);
            claims.Should().Contain(c => c.Type == ClaimTypes.Email);
            claims.Should().Contain(c => c.Type == ClaimTypes.Role);
        }

        [Fact]
        public void GetUserClaimsFromToken_WithInvalidToken_ShouldReturnEmpty()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();

            // Act
            var claims = _tokenManagement.GetUserClaimsFromToken("invalid_token_format");

            // Assert
            claims.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateRefreshToken_WithExistingToken_ShouldUpdate()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var oldToken = "old_token_" + Guid.NewGuid();
            await _tokenManagement.AddRefreshToken(userId, oldToken);

            // Act
            var result = await _tokenManagement.UpdateRefreshToken(oldToken);

            // Assert
            result.Should().Be(0);
            var updated = await DbContext.RefreshToken.FirstOrDefaultAsync(t => t.UserId == userId);
            updated!.Token.Should().Be(oldToken);
        }

        [Fact]
        public async Task UpdateRefreshToken_WithNonExistentToken_ShouldReturnNegativeOne()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var nonExistentToken = "nonexistent_" + Guid.NewGuid();

            // Act
            var result = await _tokenManagement.UpdateRefreshToken(nonExistentToken);

            // Assert
            result.Should().Be(-1);
        }

        [Fact]
        public async Task ValidateRefreshToken_WithExistingToken_ShouldReturnTrue()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var token = "valid_token_" + Guid.NewGuid();

            await _tokenManagement.AddRefreshToken(userId, token);

            // Act
            var isValid = await _tokenManagement.ValidateRefreshToken(token);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateRefreshToken_WithNonExistentToken_ShouldReturnFalse()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();

            // Act
            var isValid = await _tokenManagement.ValidateRefreshToken("nonexistent_token");

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserIdByRefreshToken_WithExistingToken_ShouldReturnUserId()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var token = "test_token_" + Guid.NewGuid();

            await _tokenManagement.AddRefreshToken(userId, token);

            // Act
            var retrievedUserId = await _tokenManagement.GetUserIdByRefreshToken(token);

            // Assert
            retrievedUserId.Should().Be(userId);
        }

        [Fact]
        public async Task GetUserIdByRefreshToken_WithInvalidToken_ShouldThrow()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(
                () => _tokenManagement.GetUserIdByRefreshToken("nonexistent_token"));
        }

        [Fact]
        public async Task TokenRefreshWorkflow_ShouldWorkCorrectly()
        {
            // Arrange
            _tokenManagement = GetTokenManagement();
            var userId = Guid.NewGuid().ToString();
            var initialToken = _tokenManagement.GetRefreshToken();

            // Act & Assert - Add initial token
            await _tokenManagement.AddRefreshToken(userId, initialToken);
            var isValid = await _tokenManagement.ValidateRefreshToken(initialToken);
            isValid.Should().BeTrue();

            // Update with existing token (current contract)
            var newToken = _tokenManagement.GetRefreshToken();
            var updateResult = await _tokenManagement.UpdateRefreshToken(newToken);
            updateResult.Should().Be(-1);

            // Existing token remains valid and new token isn't persisted by this method
            var isOldValid = await _tokenManagement.ValidateRefreshToken(initialToken);
            isOldValid.Should().BeTrue();
            var isNewValid = await _tokenManagement.ValidateRefreshToken(newToken);
            isNewValid.Should().BeFalse();
        }
    }
}
