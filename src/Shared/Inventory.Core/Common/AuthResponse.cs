using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Common
{
    public class AuthResponse
    {
        public ResponeStatus Status { get; set; }
        public IList<ResponseMessage>? Messages { get; set; }
        public TokenModel? Token { get; set; }
    }
}
