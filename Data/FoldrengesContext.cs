using Microsoft.EntityFrameworkCore;
using Földrengések2026.Models;

namespace Földrengések2026.Data
{
    public class FoldrengesContext
    {
        public FoldrengesContext(DbContextOptions<FoldrengesContext> options) : base(options)
        {
        }
        public DbSet<Models.Telepules> Telepulesek { get; set; } = null!;

        public DbSet<Models.Naplo> Naplok { get; set; } = null!;
    }
}
