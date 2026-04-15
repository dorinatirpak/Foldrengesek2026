using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Földrengések2026.Data;
using Földrengések2026.Models;

namespace Földrengések2026.Controllers
{
    public class NaploController : Controller
    {
        private readonly FoldrengesContext _context;

        public NaploController(FoldrengesContext context)
        {
            _context = context;
        }

        // GET: Naplo
        public async Task<IActionResult> Index(DateTime? datum, int? telepulesid, double? minMagnitudo, double? maxMagnitudo, string sort = "datum", string dir = "desc")
        {
            var foldrengesek = _context.Naplok.Include(n => n.Telepules).AsQueryable();

            // Dátum szűrés
            if (datum.HasValue)
            {
                foldrengesek = foldrengesek.Where(n => n.Datum == datum);
                ViewData["AktualisDatumSzuro"] = datum.Value.ToString("yyyy-MM-dd");
            }

            // Település szűrés
            if (telepulesid != null && telepulesid > 0)
            {
                foldrengesek = foldrengesek.Where(b => b.TelepulesID == telepulesid);
                ViewData["AktualisTelepulesSzuro"] = telepulesid; 
            }

            // Minimum magnitúdó szűrés
            if (minMagnitudo.HasValue)
            {
                foldrengesek = foldrengesek.Where(n => n.Magnitudo >= minMagnitudo);
                ViewData["MinMagnitudo"] = minMagnitudo;
            }

            // Maximum magnitúdó szűrés
            if (maxMagnitudo.HasValue)
            {
                foldrengesek = foldrengesek.Where(n => n.Magnitudo <= maxMagnitudo);
                ViewData["MaxMagnitudo"] = maxMagnitudo;
            }

            // Rendezés beállítása
            foldrengesek = (sort, dir) switch
            {
                ("datum", "asc") => foldrengesek.OrderBy(n => n.Datum).ThenBy(n => n.Ido),
                ("datum", "desc") => foldrengesek.OrderByDescending(n => n.Datum).ThenByDescending(n => n.Ido),
                ("magnitudo", "asc") => foldrengesek.OrderBy(n => n.Magnitudo),
                ("magnitudo", "desc") => foldrengesek.OrderByDescending(n => n.Magnitudo),
                ("intenzitas", "asc") => foldrengesek.OrderBy(n => n.Intenzitas),
                ("intenzitas", "desc") => foldrengesek.OrderByDescending(n => n.Intenzitas),
                ("telepules", "asc") => foldrengesek.OrderBy(n => n.Telepules.Nev),
                ("telepules", "desc") => foldrengesek.OrderByDescending(n => n.Telepules.Nev),
                _ => foldrengesek.OrderByDescending(n => n.Datum)
            };

            ViewData["CurrentSort"] = sort;
            ViewData["CurrentDir"] = dir;

            int totalCount = await foldrengesek.CountAsync();
            ViewData["TotalCount"] = totalCount;

            ViewData["TelepulesID"] = new SelectList(
                _context.Telepulesek,
                "ID",
                "Nev",
                telepulesid ?? 0
            );

            return View(await foldrengesek.ToListAsync());
        }

        // GET: Naplo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok
                .Include(n => n.Telepules)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (naplo == null)
            {
                return NotFound();
            }

            return View(naplo);
        }

        // GET: Naplo/Create
        public IActionResult Create()
        {
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev");
            return View();
        }

        // POST: Naplo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Datum,Ido,Magnitudo,Intenzitas,TelepulesID")] Naplo naplo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(naplo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // GET: Naplo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok.FindAsync(id);
            if (naplo == null)
            {
                return NotFound();
            }
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // POST: Naplo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Datum,Ido,Magnitudo,Intenzitas,TelepulesID")] Naplo naplo)
        {
            if (id != naplo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(naplo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NaploExists(naplo.ID))
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
            ViewData["TelepulesID"] = new SelectList(_context.Telepulesek, "ID", "Nev", naplo.TelepulesID);
            return View(naplo);
        }

        // GET: Naplo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var naplo = await _context.Naplok
                .Include(n => n.Telepules)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (naplo == null)
            {
                return NotFound();
            }

            return View(naplo);
        }

        // POST: Naplo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var naplo = await _context.Naplok.FindAsync(id);
            if (naplo != null)
            {
                _context.Naplok.Remove(naplo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NaploExists(int id)
        {
            return _context.Naplok.Any(e => e.ID == id);
        }
    }
}
