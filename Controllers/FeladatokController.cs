using Microsoft.AspNetCore.Mvc;
using Földrengések2026.Data;
using Földrengések2026.ViewModels;
using System.Linq;

namespace Földrengések2026.Controllers
{
    public class FeladatokController : Controller
    {
        private readonly FoldrengesContext _context;

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
            var results = _context.Telepulesek
                .Where(t => t.Varmegye == "Somogy")
                .OrderBy(t => t.Nev)
                .Select(t => t.Nev);

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
    }
}