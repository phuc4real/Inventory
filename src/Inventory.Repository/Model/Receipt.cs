using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Receipt : BaseModel
    {
        public int ItemCount { get; set; }
        public int Total { get; set;}
    }
}
