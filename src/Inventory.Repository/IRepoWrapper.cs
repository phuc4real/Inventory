﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository
{
    public interface IRepoWrapper
    {
        Task SaveAsync();

        ICategoryRepository Category { get; }
        ICommentRepository Comment { get; }
        IExportEntryRepository ExportEntry { get; }
        IExportRepository Export { get; }
        IItemRepository Item { get; }
        IOrderEntryRepository OrderEntry { get; }
        IOrderRepository Order { get; }
        IOrderRecordRepository OrderRecord { get; }
        IStatusRepository Status { get; }
        ITicketRepository Ticket { get; }
        ITicketEntryRepository TicketEntry { get; }
        ITicketRecordRepository TicketRecord { get; }
        ITicketTypeRepository TicketType { get; }
    }
}
