using Inventory.Service.Common;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketTypeResponse
    {
        public int Id { get; set; }
        public string? Description { get; set; }
    }
    public class TicketTypeList : PaginationResponse<TicketTypeResponse>
    {

    }
}
