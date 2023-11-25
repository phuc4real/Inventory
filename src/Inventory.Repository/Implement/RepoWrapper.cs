using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Implement
{
    public class RepoWrapper : IRepoWrapper
    {
        #region Ctor

        private readonly AppDbContext _context;
        private string? _userContext;

        public RepoWrapper(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Method
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void SetUserContext(string userContext)
        {
            _userContext = userContext;
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

        #region Interface

        public ICategoryRepository Category
        {
            get
            {
                _category ??= new CategoryRepository(_context);
                _category.SetUserContext(_userContext);
                return _category;
            }
        }
        public ICommentRepository Comment
        {
            get
            {
                _comment ??= new CommentRepository(_context);
                _comment.SetUserContext(_userContext);
                return _comment;
            }
        }
        public IExportEntryRepository ExportEntry
        {
            get
            {
                _exportEntry ??= new ExportEntryRepository(_context);
                _exportEntry.SetUserContext(_userContext);
                return _exportEntry;
            }
        }
        public IExportRepository Export
        {
            get
            {
                _export ??= new ExportRepository(_context);
                _export.SetUserContext(_userContext);
                return _export;
            }
        }
        public IItemRepository Item
        {
            get
            {
                _item ??= new ItemRepository(_context);
                _item.SetUserContext(_userContext);
                return _item;
            }
        }
        public IOrderEntryRepository OrderEntry
        {
            get
            {
                _orderEntry ??= new OrderEntryRepository(_context);
                _orderEntry.SetUserContext(_userContext);
                return _orderEntry;
            }
        }
        public IOrderRecordRepository OrderRecord
        {
            get
            {
                _orderRecord ??= new OrderRecordRepository(_context);
                _orderRecord.SetUserContext(_userContext);
                return _orderRecord;
            }
        }
        public IOrderRepository Order
        {
            get
            {
                _order ??= new OrderRepository(_context);
                _order.SetUserContext(_userContext);
                return _order;
            }
        }
        public IStatusRepository Status
        {
            get
            {
                _status ??= new StatusRepository(_context);
                _status.SetUserContext(_userContext);
                return _status;
            }
        }
        public ITicketRepository Ticket
        {
            get
            {
                _ticket ??= new TicketRepository(_context);
                _ticket.SetUserContext(_userContext);
                return _ticket;
            }
        }
        public ITicketEntryRepository TicketEntry
        {
            get
            {
                _ticketEntry ??= new TicketEntryRepository(_context);
                _ticketEntry.SetUserContext(_userContext);
                return _ticketEntry;
            }
        }
        public ITicketRecordRepository TicketRecord
        {
            get
            {
                _ticketRecord ??= new TicketRecordRepository(_context);
                _ticketRecord.SetUserContext(_userContext);
                return _ticketRecord;
            }
        }
        public ITicketTypeRepository TicketType
        {
            get
            {
                _ticketType ??= new TicketTypeRepository(_context);
                _ticketType.SetUserContext(_userContext);
                return _ticketType;
            }
        }

        public IQueryable<AppUser> User
        {
            get
            {
                return _context.Users.AsQueryable();
            }
        }

        public IQueryable<IdentityUserRole<string>> UserRole
        {
            get
            {
                return _context.UserRoles.AsQueryable();
            }
        }

        public IQueryable<IdentityRole> Role
        {
            get
            {
                return _context.Roles.AsQueryable();
            }
        }

        #endregion
    }
}
