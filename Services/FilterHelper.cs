using System.Linq.Expressions;

namespace Földrengések2026.Services
{
    public static class FilterHelper
    {
        /// <summary>
        /// Applies a case-insensitive string contains filter to the query.
        /// If filterValue is null or empty, the query is returned unchanged.
        /// </summary>
        public static IQueryable<T> WhereContains<T>(
            this IQueryable<T> query,
            string? filterValue,
            Expression<Func<T, string>> propertySelector)
        {
            if (string.IsNullOrEmpty(filterValue))
                return query;

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;

            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

            var propertyToLower = Expression.Call(property, toLowerMethod);
            var filterValueLower = Expression.Constant(filterValue.ToLower());
            var containsCall = Expression.Call(propertyToLower, containsMethod, filterValueLower);

            var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);
            return query.Where(lambda);
        }

        /// <summary>
        /// Applies an exact equality filter for nullable value types.
        /// If filterValue is null, the query is returned unchanged.
        /// </summary>
        public static IQueryable<T> WhereEquals<T, TValue>(
            this IQueryable<T> query,
            TValue? filterValue,
            Expression<Func<T, TValue>> propertySelector) where TValue : struct
        {
            if (!filterValue.HasValue)
                return query;

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            var constant = Expression.Constant(filterValue.Value, typeof(TValue));
            var equals = Expression.Equal(property, constant);

            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
            return query.Where(lambda);
        }

        /// <summary>
        /// Applies a minimum/maximum range filter on a double property.
        /// Either min or max (or both) can be null; null values are skipped.
        /// </summary>
        public static IQueryable<T> WhereInRange<T>(
            this IQueryable<T> query,
            double? min,
            double? max,
            Expression<Func<T, double>> propertySelector)
        {
            if (min.HasValue)
            {
                var parameter = propertySelector.Parameters[0];
                var property = propertySelector.Body;
                var minConstant = Expression.Constant(min.Value);
                var greaterOrEqual = Expression.GreaterThanOrEqual(property, minConstant);
                var lambda = Expression.Lambda<Func<T, bool>>(greaterOrEqual, parameter);
                query = query.Where(lambda);
            }

            if (max.HasValue)
            {
                var parameter = propertySelector.Parameters[0];
                var property = propertySelector.Body;
                var maxConstant = Expression.Constant(max.Value);
                var lessOrEqual = Expression.LessThanOrEqual(property, maxConstant);
                var lambda = Expression.Lambda<Func<T, bool>>(lessOrEqual, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        /// <summary>
        /// Applies sorting using a dictionary of named sort options.
        /// Each SortOption defines ascending and descending orderings.
        /// Falls back to defaultSort if the sort key is not found.
        /// </summary>
        public static IOrderedQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string sort,
            string dir,
            Dictionary<string, SortOption<T>> sortOptions,
            Func<IQueryable<T>, IOrderedQueryable<T>> defaultSort)
        {
            if (sortOptions.TryGetValue(sort, out var option))
            {
                return dir == "desc" ? option.Descending(query) : option.Ascending(query);
            }
            return defaultSort(query);
        }
    }
}
