
using System.ComponentModel.DataAnnotations;

namespace SafeIt.Entities
{
    public class File
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}