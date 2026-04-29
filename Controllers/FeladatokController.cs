using Földrengések2026.Data;
using Földrengések2026.Services;
using Földrengések2026.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Földrengések2026.Controllers
{
    [Authorize]
    public class FeladatokController(FoldrengesContext context, ILekerdezesiFeladatok queries) : Controller
    {
        private readonly FoldrengesContext _context = context;
        private readonly ILekerdezesiFeladatok _queries = queries;

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
            var results = _queries.VarmegyeiRengesSzamok();
            return View(results);
        }

        public IActionResult Feladat4()
        {
            var result = _queries.LegnagyobbMagnitudo();
            return View(result);
        }

        public IActionResult Feladat5()
        {
            var result = _queries.AligErzekelheto2022().ToList();
            return View(result);
        }

        public IActionResult Feladat6()
        {
            var results = _queries.Top3Ev_3nalNagyobbIntenzitassal().ToList();
            return View(results);
        }
    }
}