using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Implement
{
    public class ExportRepository : BaseRepository<Export>, IExportRepository
    {
        public ExportRepository(AppDbContext context) : base(context)
        {
        }
    }
}
