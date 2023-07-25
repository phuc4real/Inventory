using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class Login
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
