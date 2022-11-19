using System.ComponentModel.DataAnnotations;
namespace SafeIt.Models
{
    public class FileModel
    {

        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        [Display(Name = "File")]
        public IFormFile Document { get; set; }
    }
}
