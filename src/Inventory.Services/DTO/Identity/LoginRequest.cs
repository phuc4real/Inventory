﻿using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Identity
{
    public class LoginRequest : BaseRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
