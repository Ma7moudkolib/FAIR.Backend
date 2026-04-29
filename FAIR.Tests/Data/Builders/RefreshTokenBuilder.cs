using Bogus;
using FAIR.Domain.Entities.Identity;

namespace FAIR.Tests.Data.Builders
{
    /// <summary>
    /// Builder for creating RefreshToken test entities.
    /// </summary>
    public class RefreshTokenBuilder
    {
        private readonly Faker<RefreshToken> _faker;
        private RefreshToken _refreshToken;

        public RefreshTokenBuilder()
        {
            _faker = new Faker<RefreshToken>()
                .RuleFor(rt => rt.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(rt => rt.Token, f => f.Random.AlphaNumeric(64))
                .RuleFor(rt => rt.UserId, _ => Guid.NewGuid().ToString());

            _refreshToken = _faker.Generate();
        }

        public RefreshTokenBuilder WithUserId(string userId)
        {
            _refreshToken.UserId = userId;
            return this;
        }

        public RefreshTokenBuilder WithToken(string token)
        {
            _refreshToken.Token = token;
            return this;
        }

        public RefreshToken Build()
        {
            return _refreshToken;
        }
    }
}
