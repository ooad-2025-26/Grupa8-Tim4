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
    public class TortasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TortasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tortas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Torta.ToListAsync());
        }

        // GET: Tortas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var torta = await _context.Torta
                .FirstOrDefaultAsync(m => m.TortaID == id);
            if (torta == null)
            {
                return NotFound();
            }

            return View(torta);
        }

        // GET: Tortas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tortas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TortaID,Naziv,Cijena,KolicinaNaStanju,Dostupna")] Torta torta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(torta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(torta);
        }

        // GET: Tortas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var torta = await _context.Torta.FindAsync(id);
            if (torta == null)
            {
                return NotFound();
            }
            return View(torta);
        }

        // POST: Tortas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TortaID,Cijena,Naziv,KolicinaNaStanju,Dostupna")] Torta torta)
        {
            if (id != torta.TortaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(torta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TortaExists(torta.TortaID))
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
            return View(torta);
        }

        // GET: Tortas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var torta = await _context.Torta
                .FirstOrDefaultAsync(m => m.TortaID == id);
            if (torta == null)
            {
                return NotFound();
            }

            return View(torta);
        }

        // POST: Tortas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var torta = await _context.Torta.FindAsync(id);
            if (torta != null)
            {
                _context.Torta.Remove(torta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TortaExists(int id)
        {
            return _context.Torta.Any(e => e.TortaID == id);
        }
    }
}
