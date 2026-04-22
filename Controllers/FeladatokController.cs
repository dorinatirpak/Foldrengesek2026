using Földrengések2026.Data;
using Földrengések2026.Services;
using Földrengések2026.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Földrengések2026.Controllers
{
    public class FeladatokController : Controller
    {
        private readonly FoldrengesContext _context;
        private readonly ILekerdezesiFeladatok _queries;

        public FeladatokController(FoldrengesContext context, ILekerdezesiFeladatok queries)
        {
            _context = context;
            _queries = queries;
        }

        public FeladatokController(FoldrengesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Feladat2()
        {
            var results = _queries.SomogyTelepulesNevek();
            return View(results);
        }

        public IActionResult Feladat3()
        {
            var results = _context.Telepulesek
                .Join(_context.Naplok,
                    telepules => telepules.ID,
                    naplo => naplo.TelepulesID,
                    (telepules, naplo) => new
                    {
                        telepules.Varmegye
                    })
                .GroupBy(t => t.Varmegye)
                .Select(g => new Feladat3ViewModel
                {
                    Varmegye = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count);

            return View(results);
        }
        public IActionResult Feladat4()
        {
            var result = _context.Naplok
                .Select(n => new Feladat4ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Ido = n.Ido,
                    Magnitudo = n.Magnitudo
                })
                .OrderByDescending(x => x.Magnitudo)
                .FirstOrDefault();

            return View(result);
        }

        public IActionResult Feladat5()
        {
            var result = _context.Naplok
                .Where(n => n.Datum.Year == 2022
                         && n.Intenzitas >= 2.0
                         && n.Intenzitas <= 3.0)
                .OrderBy(n => n.Datum)
                .Select(n => new Feladat5ViewModel
                {
                    Nev = n.Telepules!.Nev,
                    Datum = n.Datum,
                    Intenzitas = n.Intenzitas
                })
                .ToList();

            return View(result);
        }

        public IActionResult Feladat6()
        {
            var results = _context.Naplok
                .Where(n => n.Intenzitas > 3)
                .GroupBy(n => n.Datum.Year)
                .Select(g => new Feladat6ViewModel
                {
                    Year = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .ToList();

            return View(results);
        }
    }
}