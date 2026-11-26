using Lageros.Data;
using Lageros.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class SektoriController : Controller
{
    private readonly ApplicationDbContext _context;

    public SektoriController(ApplicationDbContext context)
    {
        _context = context;
    }

    // READ: lista
    public async Task<IActionResult> Index()
    {
        var sektori = await _context.Sektori
            .Include(s => s.Zaposlenici)
            .OrderBy(s => s.Naziv)
            .ToListAsync();
        return View(sektori);
    }

    // CREATE: GET
    public IActionResult Create()
    {
        return View();
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sektor sektor)
    {
        if (!ModelState.IsValid)
        {
            return View(sektor);
        }

        _context.Add(sektor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // EDIT: GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var sektor = await _context.Sektori.FindAsync(id);
        if (sektor == null) return NotFound();

        return View(sektor);
    }

    // EDIT: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sektor sektor)
    {
        if (id != sektor.Id) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(sektor);
        }

        try
        {
            _context.Update(sektor);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Sektori.AnyAsync(s => s.Id == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    // DELETE: GET (potvrda)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var sektor = await _context.Sektori
            .Include(s => s.Zaposlenici)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sektor == null) return NotFound();

        return View(sektor);
    }

    // DELETE: POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sektor = await _context.Sektori
            .Include(s => s.Zaposlenici)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sektor == null) return NotFound();

        if (sektor.Zaposlenici.Any())
        {
            TempData["Greska"] = "Sektor ima zaposlenike i ne može se obrisati.";
            return RedirectToAction(nameof(Index));
        }

        _context.Sektori.Remove(sektor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}