using System.ComponentModel.DataAnnotations;

namespace Inventory.API.ViewModel
{
    public class RegisterDTO
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
