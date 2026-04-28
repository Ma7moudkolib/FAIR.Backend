using System.Text.Json.Serialization;

namespace FAIR.Application.DTOs.Video
{
    public class AiModelResponseDto
    {
        [JsonPropertyName("analysis_id")]
        public string? AnalysisId { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("raw_result")]
        public string? RawResult { get; set; }

        [JsonPropertyName("score")]
        public decimal? Score { get; set; }

        [JsonPropertyName("metrics")]
        public AiModelMetricsDto? Metrics { get; set; }

        public string RawJson { get; set; } = string.Empty;
    }

    public class AiModelMetricsDto
    {
        [JsonPropertyName("overall_score")]
        public decimal OverallScore { get; set; }

        [JsonPropertyName("confidence")]
        public decimal? Confidence { get; set; }

        [JsonPropertyName("movement")]
        public AiModelMovementDto? Movement { get; set; }
    }

    public class AiModelMovementDto
    {
        [JsonPropertyName("speed")]
        public decimal? Speed { get; set; }

        [JsonPropertyName("stability")]
        public decimal? Stability { get; set; }
    }
}
