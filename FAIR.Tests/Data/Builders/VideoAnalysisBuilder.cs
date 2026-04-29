using Bogus;
using FAIR.Domain.Entities;
using FAIR.Domain.Enums;

namespace FAIR.Tests.Data.Builders
{
    /// <summary>
    /// Builder for creating VideoAnalysis test entities with realistic data.
    /// </summary>
    public class VideoAnalysisBuilder
    {
        private readonly Faker<VideoAnalysis> _faker;
        private VideoAnalysis _videoAnalysis;

        public VideoAnalysisBuilder()
        {
            _faker = new Faker<VideoAnalysis>()
                .RuleFor(v => v.Id, _ => Guid.NewGuid())
                .RuleFor(v => v.AthleteId, f => f.Random.Hash(40)) // Random 40-char string hash for user ID
                .RuleFor(v => v.Score, f => f.Random.Decimal(0, 100)) // precision 6,2
                .RuleFor(v => v.ScorePercentage, f => f.Random.Decimal(0, 1)) // 0-1 scale
                .RuleFor(v => v.AvgShotSpeed, f => f.Random.Decimal(60, 200)) // km/h
                .RuleFor(v => v.AvgSpeed, f => f.Random.Decimal(5, 25)) // km/h
                .RuleFor(v => v.MaxAcceleration, f => f.Random.Decimal(5, 20)) // m/s²
                .RuleFor(v => v.MaxShotInconsistance, f => f.Random.Decimal(0, 10)) // percentage
                .RuleFor(v => v.MaxDistanceCovered, f => f.Random.Decimal(1000, 5000)) // meters
                .RuleFor(v => v.MaxRallyContribution, f => f.Random.Decimal(0, 100)) // percentage
                .RuleFor(v => v.AiResultRaw, f => f.Lorem.Paragraphs(2))
                .RuleFor(v => v.AiSummary, f => f.Lorem.Sentence())
                .RuleFor(v => v.AiRawResponse, f => f.Lorem.Paragraphs(1))
                .RuleFor(v => v.ProcessingStatus, f => f.PickRandom(new[] { 
                    AnalysisProcessingStatus.Pending, 
                    AnalysisProcessingStatus.Processing, 
                    AnalysisProcessingStatus.Completed, 
                    AnalysisProcessingStatus.Failed 
                }))
                .RuleFor(v => v.CreatedDate, f => f.Date.Recent());

            _videoAnalysis = _faker.Generate();
        }

        public VideoAnalysisBuilder WithAthleteId(string athleteId)
        {
            _videoAnalysis.AthleteId = athleteId;
            return this;
        }

        public VideoAnalysisBuilder WithScore(decimal score)
        {
            _videoAnalysis.Score = Math.Clamp(score, 0, 100);
            _videoAnalysis.ScorePercentage = _videoAnalysis.Score / 100;
            return this;
        }

        public VideoAnalysisBuilder WithProcessingStatus(AnalysisProcessingStatus status)
        {
            _videoAnalysis.ProcessingStatus = status;
            return this;
        }

        public VideoAnalysisBuilder WithCreatedDate(DateTime createdDate)
        {
            _videoAnalysis.CreatedDate = createdDate;
            return this;
        }

        public VideoAnalysisBuilder WithAvgShotSpeed(decimal speed)
        {
            _videoAnalysis.AvgShotSpeed = speed;
            return this;
        }

        public VideoAnalysisBuilder WithAiResultRaw(string rawResult)
        {
            _videoAnalysis.AiResultRaw = rawResult;
            return this;
        }

        public VideoAnalysis Build()
        {
            return _videoAnalysis;
        }
    }
}
