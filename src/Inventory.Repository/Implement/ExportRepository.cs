using Inventory.Database.DbContext;
using Inventory.Model.Entity;

namespace Inventory.Repository.Implement
{
    public class ExportRepository : BaseRepository<Export>, IExportRepository
    {
        public ExportRepository(AppDbContext context) : base(context)
        {
        }
    }
}
