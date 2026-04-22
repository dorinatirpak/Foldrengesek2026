using Földrengések2026.Data;
using Földrengések2026.ViewModels;

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

        public IQueryable<Feladat3ViewModel> VarmegyeiRengesSzamok()
            => _context.Telepulesek
                .Join(_context.Naplok,
                    telepules => telepules.ID,
                    naplo => naplo.TelepulesID,
                    (telepules, naplo) => new { telepules.Varmegye })
                .GroupBy(x => x.Varmegye)
                .Select(g => new Feladat3ViewModel
                {
                    Varmegye = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count);

        public Feladat4ViewModel? LegnagyobbMagnitudo()
            => _context.Naplok
                .Select(n => new Feladat4ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Ido = n.Ido,
                    Magnitudo = n.Magnitudo
                })
                .OrderByDescending(x => x.Magnitudo)
                .FirstOrDefault();
    }
}