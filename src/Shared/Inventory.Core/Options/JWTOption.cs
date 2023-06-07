using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Options
{
    public class JWTOption
    {
        public const string JWTBearer = "JWTBearer";
        public string ValidAudience { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
