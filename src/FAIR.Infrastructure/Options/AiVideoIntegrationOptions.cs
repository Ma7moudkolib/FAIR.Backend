namespace FAIR.Infrastructure.Options
{
    public class AiVideoIntegrationOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = "video-analysis-v1";
        public string Endpoint { get; set; } = "/api/analyze-video";
        public string VideoFieldName { get; set; } = "video";
        public int TimeoutSeconds { get; set; } = 120;
    }
}
