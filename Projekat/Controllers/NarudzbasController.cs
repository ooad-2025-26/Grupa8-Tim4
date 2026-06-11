<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc;
=======
﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
>>>>>>> iman
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BentoLab.Data;
using BentoLab.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BentoLab.Controllers
{
    public class NarudzbasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public NarudzbasController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
<<<<<<< HEAD
            // Privremeno dok login nije gotov
            int trenutniKorisnikId = 1;

            var narudzbe = await _context.Narudzbe
                .Where(n => n.KorisnikID == trenutniKorisnikId)
                .ToListAsync();

            return View(narudzbe);
=======
            var narudzbe = _context.Narudzbe
            .Include(n => n.Korisnik);

            return View(await narudzbe.ToListAsync());
>>>>>>> iman
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe
<<<<<<< HEAD
                .FirstOrDefaultAsync(n => n.NarudzbaID == id);

            if (narudzba == null)
            {
                return NotFound();
            }
=======
            .Include(n => n.Korisnik)
            .Include(n => n.Stavke)
            .ThenInclude(s => s.Torta)
            .FirstOrDefaultAsync(m => m.NarudzbaID == id);

            if (narudzba == null) return NotFound();
>>>>>>> iman

            return View(narudzba);
        }

        public IActionResult Create()
        {
<<<<<<< HEAD
            var model = new Narudzba
            {
                DatumNarudzbe = DateTime.Today,
                Status = StatusNarudzbe.KREIRANA
            };

            return View(model);
        }

        // POST: Narudzbas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatumNarudzbe,NacinPreuzimanja")] Narudzba narudzba)
        {
            // Privremeno dok login ne bude implementiran
            narudzba.KorisnikID = 1;
            narudzba.Status = StatusNarudzbe.KREIRANA;
            narudzba.KoeficijentSlozenosti = 1.0;
            narudzba.UkupnaCijena = 30.0;

            // Provjera datuma
            if (narudzba.DatumNarudzbe.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    "DatumNarudzbe",
                    "Nije moguće odabrati datum iz prošlosti."
                );
            }

            // Maksimalno 5 narudžbi po danu
            var brojNarudzbiZaTajDan = await _context.Narudzbe
                .CountAsync(n => n.DatumNarudzbe.Date == narudzba.DatumNarudzbe.Date);

            if (brojNarudzbiZaTajDan >= 5)
            {
                ModelState.AddModelError(
                    "DatumNarudzbe",
                    "Kapacitet slastičarne za ovaj datum je popunjen! Odaberite drugi dan."
                );
            }
=======
            var narudzba = new Narudzba
            {
                DatumPreuzimanja = DateTime.Now.AddDays(7)
            };

            return View(narudzba);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatumPreuzimanja,NacinPreuzimanja")] Narudzba narudzba)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            narudzba.KorisnikID = korisnik.Id;
            narudzba.DatumNarudzbe = DateTime.UtcNow;
            narudzba.DatumPreuzimanja = DateTime.SpecifyKind(narudzba.DatumPreuzimanja, DateTimeKind.Utc);
            narudzba.Status = StatusNarudzbe.KREIRANA;
            narudzba.UkupnaCijena = 0;
            narudzba.KoeficijentSlozenosti = 1;

            ModelState.Remove("Korisnik");
            ModelState.Remove("Stavke");
            ModelState.Remove("Dostava");
            ModelState.Remove("Obavjestenja");
>>>>>>> iman

            if (ModelState.IsValid)
            {
                _context.Add(narudzba);
                await _context.SaveChangesAsync();
<<<<<<< HEAD

                // Ako je izabrana dostava
                if (narudzba.NacinPreuzimanja == NacinPreuzimanja.DOSTAVA)
                {
                    return RedirectToAction(
                        "Create",
                        "Dostavas",
                        new { id = narudzba.NarudzbaID }
                    );
                }

                // Ako je lično preuzimanje
                return RedirectToAction(nameof(Index));
            }

            return View(narudzba);
        }
=======
                return RedirectToAction("Create", "StavkaNarudzbes");
            }

            return View(narudzba);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe.FindAsync(id);
            if (narudzba == null) return NotFound();

            return View(narudzba);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NarudzbaID,UkupnaCijena,KoeficijentSlozenosti,Status,DatumNarudzbe,DatumPreuzimanja,NacinPreuzimanja,KorisnikID")] Narudzba narudzba)
        {
            if (id != narudzba.NarudzbaID) return NotFound();

            ModelState.Remove("Korisnik");
            ModelState.Remove("Stavke");
            ModelState.Remove("Dostava");
            ModelState.Remove("Obavjestenja");

            narudzba.DatumNarudzbe = DateTime.SpecifyKind(narudzba.DatumNarudzbe, DateTimeKind.Utc);
            narudzba.DatumPreuzimanja = DateTime.SpecifyKind(narudzba.DatumPreuzimanja, DateTimeKind.Utc);

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
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(narudzba);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe
            .Include(n => n.Korisnik)
            .FirstOrDefaultAsync(m => m.NarudzbaID == id);

            if (narudzba == null) return NotFound();

            return View(narudzba);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var narudzba = await _context.Narudzbe.FindAsync(id);

            if (narudzba != null)
                _context.Narudzbe.Remove(narudzba);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NarudzbaExists(int id)
        {
            return _context.Narudzbe.Any(e => e.NarudzbaID == id);
        }
>>>>>>> iman
    }
}