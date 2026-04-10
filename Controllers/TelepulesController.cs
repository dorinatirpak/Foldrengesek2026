using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Földrengések2026.Data;
using Földrengések2026.Models;
using Földrengések2026.Services;

namespace Földrengések2026.Controllers
{
    public class TelepulesController : Controller
    {
        private readonly FoldrengesContext _context;

        private static readonly Dictionary<string, SortOption<Telepules>> TelepulesSortOptions = new()
        {
            ["nev"] = new SortOption<Telepules>(
                q => q.OrderBy(p => p.Nev),
                q => q.OrderByDescending(p => p.Nev)),
            ["varmegye"] = new SortOption<Telepules>(
                q => q.OrderBy(p => p.Varmegye),
                q => q.OrderByDescending(p => p.Varmegye))
        };

        public TelepulesController(FoldrengesContext context)
        {
            _context = context;
        }

        // GET: Telepules
        public async Task<IActionResult> Index(string? nev, string? varmegye, int page = 1, string sort = "nev", string dir = "asc")
        {
            var telepulesek = _context.Telepulesek.AsQueryable();

            // Név szűrés
            telepulesek = telepulesek.WhereContains(nev, p => p.Nev!);
            if (!string.IsNullOrEmpty(nev)) ViewData["AktualisNevSzuro"] = nev;

            // Vármegye szűrés
            telepulesek = telepulesek.WhereContains(varmegye, p => p.Varmegye!);
            if (!string.IsNullOrEmpty(varmegye)) ViewData["AktualisVarmegyeSzuro"] = varmegye;

            // Rendezés beállítása
            var sorted = telepulesek.ApplySorting(sort, dir, TelepulesSortOptions,
                q => q.OrderBy(p => p.Nev));

            ViewData["CurrentSort"] = sort;
            ViewData["CurrentDir"] = dir;

            int pageSize = 10;
            int totalCount = await sorted.CountAsync();

            var items = await sorted
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalCount"] = totalCount;
            ViewData["TotalPages"] = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(items);
        }

        // GET: Telepules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek
                .FirstOrDefaultAsync(m => m.ID == id);
            if (telepules == null)
            {
                return NotFound();
            }

            return View(telepules);
        }

        // GET: Telepules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Telepules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Nev,Varmegye")] Telepules telepules)
        {
            if (ModelState.IsValid)
            {
                _context.Add(telepules);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(telepules);
        }

        // GET: Telepules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek.FindAsync(id);
            if (telepules == null)
            {
                return NotFound();
            }
            return View(telepules);
        }

        // POST: Telepules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Nev,Varmegye")] Telepules telepules)
        {
            if (id != telepules.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(telepules);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TelepulesExists(telepules.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(telepules);
        }

        // GET: Telepules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var telepules = await _context.Telepulesek
                .FirstOrDefaultAsync(m => m.ID == id);
            if (telepules == null)
            {
                return NotFound();
            }

            return View(telepules);
        }

        // POST: Telepules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var telepules = await _context.Telepulesek.FindAsync(id);
            if (telepules != null)
            {
                _context.Telepulesek.Remove(telepules);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TelepulesExists(int id)
        {
            return _context.Telepulesek.Any(e => e.ID == id);
        }
    }
}
