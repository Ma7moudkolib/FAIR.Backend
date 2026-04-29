using System.Net.Http.Headers;
using System.Text.Json;
using FAIR.Application.DTOs.Video;
using FAIR.Application.Services.Interfaces;
using FAIR.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FAIR.Application.Services.Implementations
{
    public class AiVideoService(IHttpClientFactory httpClientFactory, IOptions<AiVideoIntegrationOptions> options) : IAiVideoService
    {
        private readonly AiVideoIntegrationOptions _options = options.Value;

        public async Task<AiModelResponseDto> AnalyzeVideoAsync(Stream videoStream, string fileName, string? contentType, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_options.BaseUrl) || string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("AI video integration is not configured.");
            }

            var client = httpClientFactory.CreateClient("AiVideoIntegration");
            client.BaseAddress = new Uri(_options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            using var multipart = new MultipartFormDataContent();
            var streamContent = new StreamContent(videoStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType)
                ? "application/octet-stream"
                : contentType);
            multipart.Add(streamContent, _options.VideoFieldName, fileName);
            multipart.Add(new StringContent(_options.ModelName), "model");

            using var response = await client.PostAsync(_options.Endpoint, multipart, cancellationToken);
            var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
            response.EnsureSuccessStatusCode();

            var dto = JsonSerializer.Deserialize<AiModelResponseDto>(
                rawJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AiModelResponseDto();

            dto.RawJson = rawJson;
            return dto;
        }
    }
}
