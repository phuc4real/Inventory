﻿using Inventory.Service.Common.Response;
using Inventory.Service.DTO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Export
{
    public class ExportEntryResponse
    {
        public int Id { get; set; }
        public int ExportId { get; set; }
        public ItemResponse? Item { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    public class ExportEntryListResponse : ListResponse<ExportEntryResponse> { }
}
