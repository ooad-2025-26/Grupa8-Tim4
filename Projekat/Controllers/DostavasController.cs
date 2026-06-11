using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;
using System.Threading.Tasks;

namespace BentoLab.Controllers
{
    public class DostavasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DostavasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dostavas/Create/5
        public IActionResult Create(int id)
        {
            var narudzba = _context.Narudzbe.Find(id);

            if (narudzba == null)
            {
                return NotFound();
            }

            var model = new Dostava
            {
                NarudzbaID = id,
                CijenaDostave = 5.0,
                VrijemeIsporuke = System.DateTime.Now.AddDays(1)
            };

            return View(model);
        }

        // POST: Dostavas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Adresa,KontaktTelefon,CijenaDostave,VrijemeIsporuke,Napomena,NarudzbaID")]
            Dostava dostava)
        {
            if (ModelState.IsValid)
            {
                // Spašavanje dostave
                _context.Add(dostava);
                await _context.SaveChangesAsync();

                // Dodavanje cijene dostave na ukupnu cijenu narudžbe
                var narudzba = await _context.Narudzbe.FindAsync(dostava.NarudzbaID);

                if (narudzba != null)
                {
                    narudzba.UkupnaCijena += dostava.CijenaDostave;

                    _context.Update(narudzba);
                    await _context.SaveChangesAsync();
                }

                // Povratak na pregled narudžbi
                return RedirectToAction("Index", "Narudzbas");
            }

            return View(dostava);
        }
    }
}