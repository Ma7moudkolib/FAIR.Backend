using FAIR.Domain.Entities;
using FAIR.Domain.Entities.Chat;
using FAIR.Domain.Entities.Identity;
using FAIR.Infrastructure.Context;
using FAIR.Tests.Data.Builders;

namespace FAIR.Tests.Data
{
    /// <summary>
    /// Helper class for seeding test data using the builder pattern.
    /// Provides factory methods for creating common test scenarios.
    /// </summary>
    public static class TestDataSeeder
    {
        /// <summary>
        /// Seeds a default Athlete for testing.
        /// </summary>
        public static async Task<Athlete> SeedDefaultAthleteAsync(dbContext context, string? email = null, string? username = null)
        {
            var athlete = new AthleteBuilder()
                .WithEmail(email ?? $"athlete-{Guid.NewGuid()}@test.com")
                .WithUsername(username ?? $"athlete_{Guid.NewGuid()}")
                .Build();

            context.Athletes.Add(athlete);
            await context.SaveChangesAsync();
            return athlete;
        }

        /// <summary>
        /// Seeds multiple Athletes for testing.
        /// </summary>
        public static async Task<List<Athlete>> SeedAthletesAsync(dbContext context, int count)
        {
            var athletes = new List<Athlete>();
            for (int i = 0; i < count; i++)
            {
                var athlete = new AthleteBuilder()
                    .WithEmail($"athlete{i}-{Guid.NewGuid()}@test.com")
                    .WithUsername($"athlete_{i}_{Guid.NewGuid()}")
                    .Build();
                athletes.Add(athlete);
                context.Athletes.Add(athlete);
            }
            await context.SaveChangesAsync();
            return athletes;
        }

        /// <summary>
        /// Seeds a default Coach for testing.
        /// </summary>
        public static async Task<Coach> SeedDefaultCoachAsync(dbContext context, string? email = null, string? username = null)
        {
            var coach = new CoachBuilder()
                .WithEmail(email ?? $"coach-{Guid.NewGuid()}@test.com")
                .WithUsername(username ?? $"coach_{Guid.NewGuid()}")
                .Build();

            context.Coaches.Add(coach);
            await context.SaveChangesAsync();
            return coach;
        }

        /// <summary>
        /// Seeds multiple Coaches for testing.
        /// </summary>
        public static async Task<List<Coach>> SeedCoachesAsync(dbContext context, int count)
        {
            var coaches = new List<Coach>();
            for (int i = 0; i < count; i++)
            {
                var coach = new CoachBuilder()
                    .WithEmail($"coach{i}-{Guid.NewGuid()}@test.com")
                    .WithUsername($"coach_{i}_{Guid.NewGuid()}")
                    .Build();
                coaches.Add(coach);
                context.Coaches.Add(coach);
            }
            await context.SaveChangesAsync();
            return coaches;
        }

        /// <summary>
        /// Seeds a Message between two users.
        /// </summary>
        public static async Task<Message> SeedMessageAsync(dbContext context, string senderId, string receiverId, string? content = null)
        {
            var message = new MessageBuilder()
                .WithSenderId(senderId)
                .WithReceiverId(receiverId)
                .WithContent(content ?? "Test message content")
                .Build();

            context.Messages.Add(message);
            await context.SaveChangesAsync();
            return message;
        }

        /// <summary>
        /// Seeds multiple Messages for testing conversations.
        /// </summary>
        public static async Task<List<Message>> SeedMessagesAsync(dbContext context, string senderId, string receiverId, int count)
        {
            var messages = new List<Message>();
            for (int i = 0; i < count; i++)
            {
                var message = new MessageBuilder()
                    .WithSenderId(i % 2 == 0 ? senderId : receiverId)
                    .WithReceiverId(i % 2 == 0 ? receiverId : senderId)
                    .WithContent($"Message {i + 1}")
                    .WithCreatedDate(DateTime.UtcNow.AddMinutes(i))
                    .Build();
                messages.Add(message);
                context.Messages.Add(message);
            }
            await context.SaveChangesAsync();
            return messages;
        }

        /// <summary>
        /// Seeds VideoAnalysis for an athlete.
        /// </summary>
        public static async Task<VideoAnalysis> SeedVideoAnalysisAsync(dbContext context, string athleteId, decimal? score = null)
        {
            var analysis = new VideoAnalysisBuilder()
                .WithAthleteId(athleteId)
                .WithScore(score ?? 75.5m)
                .Build();

            context.VideoAnalyses.Add(analysis);
            await context.SaveChangesAsync();
            return analysis;
        }

        /// <summary>
        /// Seeds multiple VideoAnalyses for an athlete.
        /// </summary>
        public static async Task<List<VideoAnalysis>> SeedVideoAnalysesAsync(dbContext context, string athleteId, int count)
        {
            var analyses = new List<VideoAnalysis>();
            for (int i = 0; i < count; i++)
            {
                var analysis = new VideoAnalysisBuilder()
                    .WithAthleteId(athleteId)
                    .WithScore(50 + (i * 5))
                    .WithCreatedDate(DateTime.UtcNow.AddDays(-count + i))
                    .Build();
                analyses.Add(analysis);
                context.VideoAnalyses.Add(analysis);
            }
            await context.SaveChangesAsync();
            return analyses;
        }

        /// <summary>
        /// Seeds a RefreshToken for a user.
        /// </summary>
        public static async Task<RefreshToken> SeedRefreshTokenAsync(dbContext context, string userId)
        {
            var token = new RefreshTokenBuilder()
                .WithUserId(userId)
                .Build();

            context.RefreshToken.Add(token);
            await context.SaveChangesAsync();
            return token;
        }

        /// <summary>
        /// Seeds multiple RefreshTokens for a user (useful for testing token rotation).
        /// </summary>
        public static async Task<List<RefreshToken>> SeedRefreshTokensAsync(dbContext context, string userId, int count)
        {
            var tokens = new List<RefreshToken>();
            for (int i = 0; i < count; i++)
            {
                var token = new RefreshTokenBuilder()
                    .WithUserId(userId)
                    .WithToken($"token_{Guid.NewGuid()}")
                    .Build();
                tokens.Add(token);
                context.RefreshToken.Add(token);
            }
            await context.SaveChangesAsync();
            return tokens;
        }

        /// <summary>
        /// Seeds test data for a complete athlete profile scenario.
        /// </summary>
        public static async Task<(Athlete athlete, List<VideoAnalysis> analyses, List<Message> messages)> 
            SeedAthleteScenarioAsync(dbContext context, string? coachId = null)
        {
            var athlete = await SeedDefaultAthleteAsync(context);
            var analyses = await SeedVideoAnalysesAsync(context, athlete.Id, 3);

            var messages = coachId != null 
                ? await SeedMessagesAsync(context, coachId, athlete.Id, 5)
                : new List<Message>();

            return (athlete, analyses, messages);
        }
    }
}
