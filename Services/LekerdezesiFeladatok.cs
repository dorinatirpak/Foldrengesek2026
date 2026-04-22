using Földrengések2026.Data;

namespace Földrengések2026.Services
{
    public class LekerdezesiFeladatok : ILekerdezesiFeladatok
    {
        private readonly FoldrengesContext _context;
        public LekerdezesiFeladatok(FoldrengesContext context) => _context = context;
        public IQueryable<string> SomogyTelepulesNevek()
        => _context.Telepulesek
        .Where(t => t.Varmegye == "Somogy")
        .OrderBy(t => t.Nev)
        .Select(t => t.Nev);
    }
}