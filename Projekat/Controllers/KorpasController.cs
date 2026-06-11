using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;

namespace BentoLab.Controllers
{
    public class KorpasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KorpasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Korpas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Korpe.Include(k => k.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Korpas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe
                .Include(k => k.Korisnik)
                .FirstOrDefaultAsync(m => m.KorpaID == id);
            if (korpa == null)
            {
                return NotFound();
            }

            return View(korpa);
        }

        // GET: Korpas/Create
        public IActionResult Create()
        {
            ViewBag.KorisnikID = new SelectList(_context.Korisnik.ToList(), "Id", "ImePrezime");
            return View();
        }

        // POST: Korpas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KorpaID,DatumKreiranja,KorisnikID")] Korpa korpa)
        {
            ModelState.Remove("Korisnik");

            if (korpa.DatumKreiranja == default)
                korpa.DatumKreiranja = DateTime.UtcNow;
            else
                korpa.DatumKreiranja = DateTime.SpecifyKind(korpa.DatumKreiranja, DateTimeKind.Utc);

            if (ModelState.IsValid)
            {
                _context.Add(korpa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.KorisnikID = new SelectList(_context.Korisnik.ToList(), "Id", "ImePrezime", korpa.KorisnikID);
            return View(korpa);
        }

        // GET: Korpas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe.FindAsync(id);
            if (korpa == null)
            {
                return NotFound();
            }
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID", korpa.KorisnikID);
            return View(korpa);
        }

        // POST: Korpas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KorpaID,DatumKreiranja,KorisnikID")] Korpa korpa)
        {
            if (id != korpa.KorpaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(korpa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KorpaExists(korpa.KorpaID))
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
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID", korpa.KorisnikID);
            return View(korpa);
        }

        // GET: Korpas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var korpa = await _context.Korpe
                .Include(k => k.Korisnik)
                .FirstOrDefaultAsync(m => m.KorpaID == id);
            if (korpa == null)
            {
                return NotFound();
            }

            return View(korpa);
        }

        // POST: Korpas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korpa = await _context.Korpe.FindAsync(id);
            if (korpa != null)
            {
                _context.Korpe.Remove(korpa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KorpaExists(int id)
        {
            return _context.Korpe.Any(e => e.KorpaID == id);
        }
    }
}
