using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Implement
{
    public class ExportEntryRepository : BaseRepository<ExportEntry>, IExportEntryRepository
    {
        public ExportEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
