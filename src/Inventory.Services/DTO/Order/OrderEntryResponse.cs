﻿using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Order
{
    public class OrderEntryResponse
    {
        public int Id { get; set; }
        public ItemCompactResponse? Items { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
    }

    public class OrderEntryListResponse : PaginationResponse<OrderEntryResponse> { }
}
