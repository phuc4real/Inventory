﻿using System.ComponentModel.DataAnnotations;

namespace Inventory.Core.ViewModel
{
    public class Register
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
