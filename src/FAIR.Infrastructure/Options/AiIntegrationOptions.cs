namespace FAIR.Infrastructure.Options
{
    public class AiIntegrationOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = "gpt-4.1-mini";
        public string Endpoint { get; set; } = "/v1/responses";
        public int TimeoutSeconds { get; set; } = 20;
    }
}
