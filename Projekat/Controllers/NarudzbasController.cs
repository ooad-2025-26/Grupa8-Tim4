using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BentoLab.Data;
using BentoLab.Models;

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

        // PREGLED SVIH NARUDZBI (Tvoja tabela)
        public async Task<IActionResult> Index()
        {
            var narudzbe = _context.Narudzbe.Include(n => n.Korisnik);
            return View(await narudzbe.ToListAsync());
        }

        // DETALJI NARUDZBE
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe
                .Include(n => n.Korisnik)
                .Include(n => n.Stavke)
                .ThenInclude(s => s.Torta)
                .FirstOrDefaultAsync(m => m.NarudzbaID == id);

            if (narudzba == null) return NotFound();

            return View(narudzba);
        }

        // STRANICA ZA KREIRANJE (GET)
        public IActionResult Create()
        {
            var narudzba = new Narudzba
            {
                DatumPreuzimanja = DateTime.Now.AddDays(7)
            };

            return View(narudzba);
        }

        // SPASAVANJE NARUDZBE (POST) - OPCIJA B (POTPUNO POVEZANO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatumPreuzimanja,NacinPreuzimanja")] Narudzba narudzba)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            // Automatsko postavljanje sistemskih polja u pozadini
            narudzba.KorisnikID = korisnik.Id;
            narudzba.DatumNarudzbe = DateTime.UtcNow;
            narudzba.DatumPreuzimanja = DateTime.SpecifyKind(narudzba.DatumPreuzimanja, DateTimeKind.Utc);
            narudzba.Status = StatusNarudzbe.KREIRANA;
            narudzba.UkupnaCijena = 0; // Početna cijena je 0 dok korisnik ne doda torte kroz stavke
            narudzba.KoeficijentSlozenosti = 1;

            // Čišćenje modela od povezanih objekata da ne blokiraju validaciju
            ModelState.Remove("Korisnik");
            ModelState.Remove("Stavke");
            ModelState.Remove("Dostava");
            ModelState.Remove("Obavjestenja");

            // Spašavanje nove narudžbe u bazu podataka kako bi dobila svoj ID
            _context.Add(narudzba);
            await _context.SaveChangesAsync();

            // POVEZIVANJE S DRUGIM DIJELOM: Preusmjeravanje na dodavanje torti (stavki) za ovu narudžbu
            // Prosljeđujemo kreirani NarudzbaID kontroleru za stavke
            return RedirectToAction("Create", "StavkaNarudzbes", new { narudzbaId = narudzba.NarudzbaID });
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe.FindAsync(id);
            if (narudzba == null) return NotFound();

            return View(narudzba);
        }

        // EDIT (POST)
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

        // DELETE (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzbe
                .Include(n => n.Korisnik)
                .FirstOrDefaultAsync(m => m.NarudzbaID == id);

            if (narudzba == null) return NotFound();

            return View(narudzba);
        }

        // DELETE (POST)
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
    }
}