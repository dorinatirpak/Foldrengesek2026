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

        public IQueryable<Feladat5ViewModel> AligErzekelheto2022()
        {
            return _context.Naplok
                .Join(_context.Telepulesek,
                    naplo => naplo.TelepulesID,
                    telepules => telepules.ID,
                    (naplo, telepules) => new Feladat5ViewModel
                    {
                        Nev = telepules.Nev,
                        Datum = naplo.Datum,
                        Intenzitas = naplo.Intenzitas
                    })
                .Where(x =>
                    x.Datum.Year == 2022 &&
                    x.Intenzitas >= 2.0 &&
                    x.Intenzitas <= 3.0)
                .OrderBy(x => x.Datum);
        }

        public IQueryable<Feladat6ViewModel> Top3Ev_3nalNagyobbIntenzitassal()
        {
            return _context.Naplok
                .Where(n => n.Intenzitas > 3.0)
                .GroupBy(n => n.Datum.Year)
                .Select(g => new Feladat6ViewModel
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(3);
        }
    }
}