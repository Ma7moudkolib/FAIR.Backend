using FAIR.Application.DTOs.Video;
using FAIR.Application.Services.Interfaces.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FAIR.Application.DTOs.Search;

namespace FAIR.API.Controllers
{
    [Route("api/athletes")]
    [ApiController]
    [Authorize]
    public class AthleteController(IServiceManager serviceManager) : ControllerBase
    {
        [HttpPost("{id:guid}/analyze-video")]
        [RequestSizeLimit(536_870_912)]
        [RequestFormLimits(MultipartBodyLengthLimit = 536_870_912)]
        public async Task<IActionResult> AnalyzeVideo(Guid id, [FromForm] VideoUploadDto uploadDto, CancellationToken cancellationToken)
        {
            if (uploadDto.Video is null || uploadDto.Video.Length <= 0)
            {
                return BadRequest(new { error = "A non-empty video file is required." });
            }

            if (uploadDto.AthleteId == Guid.Empty)
            {
                uploadDto.AthleteId = id;
            }
            else if (uploadDto.AthleteId != id)
            {
                return BadRequest(new { error = "Route athlete id does not match form athlete id." });
            }

            try
            {
                var result = await serviceManager.VideoService.AnalyzeAsync(uploadDto, cancellationToken);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { error = "AI provider call failed.", detail = ex.Message });
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, new { error = "AI analysis timed out." });
            }
            catch (IOException ex) when (ex.Message.Contains("Multipart body length limit", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new { error = "Uploaded video is too large." });
            }
        }

        [HttpGet("{id:guid}/analysis-history")]
        public async Task<IActionResult> GetAnalysisHistory(Guid id, CancellationToken cancellationToken)
        {
            var history = await serviceManager.VideoService.GetAthleteAnalysisHistoryAsync(id, cancellationToken);
            return Ok(history);
        }

        [HttpGet("{id:guid}/analysis/{analysisId:guid}")]
        public async Task<IActionResult> GetAnalysisDetails(Guid id, Guid analysisId, CancellationToken cancellationToken)
        {
            var analysis = await serviceManager.VideoService.GetAnalysisDetailsAsync(analysisId, cancellationToken);
            if (analysis == null)
            {
                return NotFound(new { error = "Video analysis not found." });
            }

            if (analysis.AthleteId != id)
            {
                return BadRequest(new { error = "Analysis does not belong to the routed athlete." });
            }

            return Ok(analysis);
        }
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] AthleteSearchFilter filter, CancellationToken cancellationToken)
        {
            var result = await serviceManager.AthleteService.SearchAsync(filter, cancellationToken);
            return Ok(result);
        }
    }
}
