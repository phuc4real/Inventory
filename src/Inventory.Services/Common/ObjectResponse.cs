using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Common
{
    public class ObjectResponse<T> : BaseResponse where T : class
    {
        public T? Data { get; set; }
    }
}
