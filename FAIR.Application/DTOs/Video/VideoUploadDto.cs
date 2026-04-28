using Microsoft.AspNetCore.Http;

namespace FAIR.Application.DTOs.Video
{
    public class VideoUploadDto
    {
        public Guid AthleteId { get; set; }
        public required IFormFile Video { get; set; }
    }
}
