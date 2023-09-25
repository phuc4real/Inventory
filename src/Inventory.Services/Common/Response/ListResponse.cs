using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Common.Response
{
    public class ListResponse<T> : BaseResponse where T : class
    {
        public List<T>? Data { get; set; }
    }
}
