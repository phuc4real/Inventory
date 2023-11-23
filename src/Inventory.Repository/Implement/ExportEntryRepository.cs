using Inventory.Database.DbContext;
using Inventory.Model.Entity;
namespace Inventory.Repository.Implement
{
    public class ExportEntryRepository : BaseRepository<ExportEntry>, IExportEntryRepository
    {
        public ExportEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
