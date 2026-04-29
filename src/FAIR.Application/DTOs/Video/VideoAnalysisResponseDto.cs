namespace FAIR.Application.DTOs.Video
{
    public class VideoAnalysisResponseDto
    {
        public Guid Id { get; set; }
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
        public string ProcessingStatus { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
