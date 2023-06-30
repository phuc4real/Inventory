using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class PaginationList<T> where T : class
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<T>? Data { get; set; }
    }
}
