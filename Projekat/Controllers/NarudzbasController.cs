using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public NarudzbasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Narudzbas
        public async Task<IActionResult> Index()
        {
            // Privremeno dok login nije gotov
            int trenutniKorisnikId = 1;

            var narudzbe = await _context.Narudzbe
                .Where(n => n.KorisnikID == trenutniKorisnikId)
                .ToListAsync();

            return View(narudzbe);
        }

        // GET: Narudzbas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var narudzba = await _context.Narudzbe
                .FirstOrDefaultAsync(n => n.NarudzbaID == id);

            if (narudzba == null)
            {
                return NotFound();
            }

            return View(narudzba);
        }

        // GET: Narudzbas/Create
        public IActionResult Create()
        {
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

            if (ModelState.IsValid)
            {
                _context.Add(narudzba);
                await _context.SaveChangesAsync();

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
    }
}