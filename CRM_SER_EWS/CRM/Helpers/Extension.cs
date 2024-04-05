using System.Linq.Expressions;

namespace CRM_EWS.CRM.Helpers
{
    public static class Extension
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, IQuery queryObj, Dictionary<string, Expression<Func<T, object>>> columnsMap)
        {
            if (string.IsNullOrWhiteSpace(queryObj.SortBy) || !columnsMap.ContainsKey(queryObj.SortBy))
            {
                return query;
            }
            return queryObj.IsSortAscending ? query.OrderBy(columnsMap[queryObj.SortBy]) : query.OrderByDescending(columnsMap[queryObj.SortBy]);
        }
    }
}
