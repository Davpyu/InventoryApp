using Amazon.S3.Model;
using System.ComponentModel.DataAnnotations;

namespace DotNetService.Domain.File.Dto
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
        public string Path { get; set; }
        public List<Tag> TagSet { get; set; }
    }
}