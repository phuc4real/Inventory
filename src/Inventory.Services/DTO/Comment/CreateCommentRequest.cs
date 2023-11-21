using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Comment
{
    public class CreateCommentRequest : BaseRequest
    {
        public int RecordId { get; set; }
        public bool IsTicketComment { get; set; }
        public bool IsReject { get; set; }
        public string? Message { get; set; }
    }
}
