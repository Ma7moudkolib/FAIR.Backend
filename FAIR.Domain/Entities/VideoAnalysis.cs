using FAIR.Domain.Entities.Identity;
using FAIR.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAIR.Domain.Entities
{
    public class VideoAnalysis
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AthleteId { get; set; }
        public string AiResultRaw { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal ScorePercentage { get; set; }
        public decimal AvgShotSpeed { get; set; }
        public decimal AvgSpeed { get; set; }
        public decimal MaxAcceleration { get; set; }
        public decimal MaxShotInconsistance { get; set; }
        public decimal MaxDistanceCovered { get; set; }
        public decimal MaxRallyContribution { get; set; }
        public string? AiSummary { get; set; }
        public string? AiRawResponse { get; set; }
        public AnalysisProcessingStatus ProcessingStatus { get; set; } = AnalysisProcessingStatus.Pending;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(AthleteId))]
        public Player? Player { get; set; }
    }
}
