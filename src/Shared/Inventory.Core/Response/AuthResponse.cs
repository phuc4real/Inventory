using Inventory.Core.Common;
using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Response
{
    public class AuthResponse : BaseResponse
    {
        public TokenModel? Token { get; set; }
    }
}
