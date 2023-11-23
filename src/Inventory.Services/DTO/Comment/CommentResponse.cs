using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Comment
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public DateTime CommentAt { get; set; }
        public string? CommentBy { get; set; }
        public bool? IsReject { get; set; }
        public string? Message { get; set; }
    }
}
