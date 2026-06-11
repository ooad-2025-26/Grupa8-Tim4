using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;
using System.Security.Claims; // Omogućava prepoznavanje prijavljenog korisnika Amre

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
        // Prikazuje tabelu sa svim narudžbama
        public async Task<IActionResult> Index()
        {
            var narudzbe = await _context.Narudzbe
                .Include(n => n.Korisnik)
                .Include(n => n.Stavke)
                .OrderByDescending(n => n.NarudzbaID)
                .ToListAsync();

            foreach (var narudzba in narudzbe)
            {
                if (narudzba.UkupnaCijena == 0 && narudzba.Stavke != null && narudzba.Stavke.Any())
                {
                    narudzba.UkupnaCijena = narudzba.Stavke.Sum(s => s.CijenaStavke);
                }
            }

            return View(narudzbe);
        }

        // GET: Narudzbas/Create
        // Otvara formu za kreiranje nove narudžbe
        public IActionResult Create()
        {
            ViewBag.TrenutniDatum = DateTime.Now.ToString("yyyy-MM-ddTHH:mm");
            return View();
        }

        // POST: Narudzbas/Create
        // Izvršava se kada klikneš na plavo dugme "Potvrdi i kreiraj narudžbu"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DateTime datumPreuzimanja, string nacinPreuzimanjaStr)
        {
            try
            {
                // Usklađivanje sa Enumom iz baze podataka: LICNO_PREUZIMANJE ili DOSTAVA
                NacinPreuzimanja nacin = nacinPreuzimanjaStr == "DOSTAVA" ? NacinPreuzimanja.DOSTAVA : NacinPreuzimanja.LICNO_PREUZIMANJE;

                // 1. KORAK: Pokušavamo dohvatiti ID trenutno logovanog korisnika iz sesije (Identity)
                string trenutniKorisnikIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int stvarniKorisnikId = 0;

                if (!string.IsNullOrEmpty(trenutniKorisnikIdStr))
                {
                    int.TryParse(trenutniKorisnikIdStr, out stvarniKorisnikId);
                }

                // 2. KORAK: Ako sesija nije pronađena, tražimo korisnika preko emaila koji je na ekranu
                if (stvarniKorisnikId == 0)
                {
                    var korisnikPoEmailu = await _context.Users.FirstOrDefaultAsync(u => u.Email == "amerdzanic1@etf.unsa.ba");
                    if (korisnikPoEmailu != null)
                    {
                        stvarniKorisnikId = korisnikPoEmailu.Id;
                    }
                    else
                    {
                        // 3. KORAK: Sigurnosni korak - ako nema tog emaila, uzmi prvog bilo kojeg korisnika iz baze
                        var biloKojiKorisnik = await _context.Users.FirstOrDefaultAsync();
                        if (biloKojiKorisnik != null)
                        {
                            stvarniKorisnikId = biloKojiKorisnik.Id;
                        }
                        else
                        {
                            return Content("Greška: Tabela sa korisnicima (Users) je potpuno prazna u bazi!");
                        }
                    }
                }

                // Kreiramo objekat narudžbe sa ispravnim podacima i validnim KorisnikID-om
                var novaNarudzba = new Narudzba
                {
                    KorisnikID = stvarniKorisnikId,
                    DatumNarudzbe = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    DatumPreuzimanja = DateTime.SpecifyKind(datumPreuzimanja, DateTimeKind.Utc),
                    NacinPreuzimanja = nacin,
                    Status = StatusNarudzbe.KREIRANA,
                    UkupnaCijena = 0,
                    KoeficijentSlozenosti = 1.0
                };

                // Dodavanje i spašavanje u PostgreSQL bazu podataka
                _context.Narudzbe.Add(novaNarudzba);
                await _context.SaveChangesAsync();

                // POPRAVAK: Umjesto ispisa teksta ili greške, ovaj kod te automatski vraća na listu narudžbi!
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"Greška prilikom kreiranja narudžbe: {ex.Message} -> {ex.InnerException?.Message}");
            }
        }
    }
}