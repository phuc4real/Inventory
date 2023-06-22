using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class LoginDTO
    {
        [Required]
        public string? UsernameOrEmail { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
