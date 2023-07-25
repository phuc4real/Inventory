namespace Inventory.Core.Response
{
    public class PaginationList<T> where T : class
    {
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<T>? Data { get; set; }
    }
}
