using System.Linq;

namespace Földrengések2026.Services
{
    public class SortOption<T>
    {
        public Func<IQueryable<T>, IOrderedQueryable<T>> Ascending { get; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> Descending { get; }

        public SortOption(
            Func<IQueryable<T>, IOrderedQueryable<T>> ascending,
            Func<IQueryable<T>, IOrderedQueryable<T>> descending)
        {
            Ascending = ascending;
            Descending = descending;
        }
    }
}
