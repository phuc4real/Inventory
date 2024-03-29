﻿using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketResponse
    {
        public int TicketId { get; set; }
        public int RecordId { get; set; }
        public string? TicketType { get; set; }
        public int TicketTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public bool IsClosed { get; set; }
        public DateTime ClosedDate { get; set; }

        public CommentResponse? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class TicketObjectResponse : ObjectResponse<TicketResponse> {
        public List<RecordHistoryResponse>? History { get; set; }
    }

    public class TicketPageResponse : PaginationResponse<TicketResponse> { }
}
