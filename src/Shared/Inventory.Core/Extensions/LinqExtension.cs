using Inventory.Core.Common;
using Inventory.Core.Helper;
using System.Linq.Expressions;

namespace Inventory.Core.Extensions
{
    public static class LinqExtension
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, sortField);
            var exp = Expression.Lambda(prop, param);
            string method = ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }

        public static IQueryable<T> Pagination<T>(this IQueryable<T> query, PaginationRequest request)
        {
            if (request.Sort != null && request.Sort != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.Sort);

                var isAsc = request.SortDirection == "asc";

                query = query.OrderByField(columnName, isAsc);
            }

            int index = request.Index.GetValueOrDefault(0);
            int size = request.Size.GetValueOrDefault(0);

            if (size > 0)
            {
                query = query.Skip(index * size).Take(size);
            }

            return query;
        }
    }
}
