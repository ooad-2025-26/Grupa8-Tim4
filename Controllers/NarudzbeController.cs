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
    public class NarudzbeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NarudzbeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Narudzbe
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Narudzbe.Include(n => n.Korisnik);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Narudzbe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narudzba = await _context.Narudzbe
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(m => m.NarudzbaID == id);
            if (narudzba == null)
            {
                return NotFound();
            }

            return View(narudzba);
        }

        // GET: Narudzbe/Create
        public IActionResult Create()
        {
            ViewData["KorisnikID"] = new SelectList(_context.Set<Korisnik>(), "KorisnikID", "KorisnikID");
            return View();
        }

        // POST: Narudzbe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NarudzbaID,UkupnaCijena,KoeficijentSlozenosti,DatumNarudzbe,KorisnikID")] Narudzba narudzba)
        {
            narudzba.Status = StatusNarudzbe.U_IZRADI;
            narudzba.NacinPreuzimanja = NacinPreuzimanja.LICNO_PREUZIMANJE;

            if (ModelState.IsValid)
            {
                _context.Add(narudzba);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(narudzba);
        }
        // GET: Narudzbe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narudzba = await _context.Narudzbe.FindAsync(id);
            if (narudzba == null)
            {
                return NotFound();
            }
            ViewData["KorisnikID"] = new SelectList(_context.Set<Korisnik>(), "KorisnikID", "KorisnikID", narudzba.KorisnikID);
            return View(narudzba);
        }

        // POST: Narudzbe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NarudzbaID,UkupnaCijena,KoeficijentSlozenosti,Status,DatumNarudzbe,NacinPreuzimanja,KorisnikID")] Narudzba narudzba)
        {
            if (id != narudzba.NarudzbaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(narudzba);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NarudzbaExists(narudzba.NarudzbaID))
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
            ViewData["KorisnikID"] = new SelectList(_context.Set<Korisnik>(), "KorisnikID", "KorisnikID", narudzba.KorisnikID);
            return View(narudzba);
        }

        // GET: Narudzbe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narudzba = await _context.Narudzbe
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(m => m.NarudzbaID == id);
            if (narudzba == null)
            {
                return NotFound();
            }

            return View(narudzba);
        }

        // POST: Narudzbe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var narudzba = await _context.Narudzbe.FindAsync(id);
            if (narudzba != null)
            {
                _context.Narudzbe.Remove(narudzba);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NarudzbaExists(int id)
        {
            return _context.Narudzbe.Any(e => e.NarudzbaID == id);
        }
    }
}
