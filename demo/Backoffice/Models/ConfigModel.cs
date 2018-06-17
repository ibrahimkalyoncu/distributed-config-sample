using System.ComponentModel.DataAnnotations;

namespace Backoffice.Models
{
    public class ConfigModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string ApplicationName { get; set; }

        public bool IsActive { get; set; }
    }
}
