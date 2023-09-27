using Inventory.Core.Common;
using Inventory.Core.Helper;
using System.Linq.Expressions;

namespace Inventory.Core.Extensions
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }

        public static IQueryable<T> Pagination<T>(this IQueryable<T> query, PaginationRequest request, ref int totalRecord)
        {
            if (request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);

                var isAsc = request.SortDirection == "asc";

                query = query.OrderByField(columnName, isAsc);
            }

            totalRecord = query.Count();

            return query.Skip(request.PageIndex * request.PageSize)
                        .Take(request.PageSize);
        }
    }
}
