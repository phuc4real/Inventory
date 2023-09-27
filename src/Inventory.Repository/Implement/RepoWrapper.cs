using Inventory.Database.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Implement
{
    public class RepoWrapper : IRepoWrapper
    {
        #region Ctor

        private readonly AppDbContext _context;
        public RepoWrapper(AppDbContext context) => _context = context;

        #endregion

        #region Method
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Private

        private CategoryRepository? _category;
        private CommentRepository? _comment;
        private ExportEntryRepository? _exportEntry;
        private ExportRepository? _export;
        private ItemRepository? _item;
        private OrderEntryRepository? _orderEntry;
        private OrderRecordRepository? _orderRecord;
        private OrderRepository? _order;
        private StatusRepository? _status;
        private TicketEntryRepository? _ticketEntry;
        private TicketRepository? _ticket;
        private TicketRecordRepository? _ticketRecord;
        private TicketTypeRepository? _ticketType;

        #endregion

        #region Repo Interface

        public ICategoryRepository Category
        {
            get
            {
                _category ??= new CategoryRepository(_context);
                return _category;
            }
        }
        public ICommentRepository Comment
        {
            get
            {
                _comment ??= new CommentRepository(_context);
                return _comment;
            }
        }
        public IExportEntryRepository ExportEntry
        {
            get
            {
                _exportEntry ??= new ExportEntryRepository(_context);
                return _exportEntry;
            }
        }
        public IExportRepository Export
        {
            get
            {
                _export ??= new ExportRepository(_context);
                return _export;
            }
        }
        public IItemRepository Item
        {
            get
            {
                _item ??= new ItemRepository(_context);
                return _item;
            }
        }
        public IOrderEntryRepository OrderEntry
        {
            get
            {
                _orderEntry ??= new OrderEntryRepository(_context);
                return _orderEntry;
            }
        }
        public IOrderRecordRepository OrderRecord
        {
            get
            {
                _orderRecord ??= new OrderRecordRepository(_context);
                return _orderRecord;
            }
        }
        public IOrderRepository Order
        {
            get
            {
                _order ??= new OrderRepository(_context);
                return _order;
            }
        }
        public IStatusRepository Status
        {
            get
            {
                _status ??= new StatusRepository(_context);
                return _status;
            }
        }
        public ITicketRepository Ticket
        {
            get
            {
                _ticket ??= new TicketRepository(_context);
                return _ticket;
            }
        }
        public ITicketEntryRepository TicketEntry
        {
            get
            {
                _ticketEntry ??= new TicketEntryRepository(_context);
                return _ticketEntry;
            }
        }
        public ITicketRecordRepository TicketRecord
        {
            get
            {
                _ticketRecord ??= new TicketRecordRepository(_context);
                return _ticketRecord;
            }
        }
        public ITicketTypeRepository TicketType
        {
            get
            {
                _ticketType ??= new TicketTypeRepository(_context);
                return _ticketType;
            }
        }

        #endregion
    }
}
