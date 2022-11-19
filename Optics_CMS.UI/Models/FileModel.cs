using System.ComponentModel.DataAnnotations;
namespace Optics_CMS.UI.Models
{
    public class FileModel
    {

        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public IFormFile Document { get; set; }
    }
}
