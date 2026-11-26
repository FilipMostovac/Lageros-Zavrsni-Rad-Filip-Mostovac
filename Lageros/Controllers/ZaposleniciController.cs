using Lageros.Data;
using Lageros.Models;
using Lageros.Models.NewFolder;
using Lageros.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class ZaposleniciController : Controller
{
    private readonly ApplicationDbContext _context;

    public ZaposleniciController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<SelectList> DohvatiSektoreAsync(int? odabrani = null)
    {
        var sektori = await _context.Sektori.OrderBy(s => s.Naziv).ToListAsync();
        return new SelectList(sektori, "Id", "Naziv", odabrani);
    }

    // READ: lista s pretragom i filtrima
    public async Task<IActionResult> Index(string? trazi, int? sektorId, bool prikaziNeaktivne = false)
    {
        var upit = _context.Zaposlenici.AsQueryable();

        if (!prikaziNeaktivne)
            upit = upit.Where(z => z.Aktivan);

        if (sektorId.HasValue)
            upit = upit.Where(z => z.SektorId == sektorId);

        if (!string.IsNullOrWhiteSpace(trazi))
        {
            trazi = trazi.Trim();
            upit = upit.Where(z => z.Ime.Contains(trazi)
                                || z.Prezime.Contains(trazi)
                                || z.Email.Contains(trazi));
        }

        var model = new ZaposlenikIndexViewModel
        {
            Trazi = trazi,
            SektorId = sektorId,
            PrikaziNeaktivne = prikaziNeaktivne,
            Sektori = await DohvatiSektoreAsync(sektorId),
            Zaposlenici = await upit
                .OrderBy(z => z.Prezime).ThenBy(z => z.Ime)
                .Select(z => new ZaposlenikRedakViewModel
                {
                    Id = z.Id,
                    Ime = z.Ime,
                    Prezime = z.Prezime,
                    Email = z.Email,
                    Radnomjesto = z.Radnomjesto,
                    Sektor = z.Sektor!.Naziv,
                    Aktivan = z.Aktivan,
                    BrojZaduzenja = z.Zaduzenja.Count(a => a.DatumRazduzenja == null)
                })
                .ToListAsync()
        };

        return View(model);
    }

    // CREATE: GET
    public async Task<IActionResult> Create()
    {
        var vm = new ZaposlenikFormViewModel { Sektori = await DohvatiSektoreAsync() };
        return View(vm);
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ZaposlenikFormViewModel vm)
    {
        if (await _context.Zaposlenici.AnyAsync(z => z.Email == vm.Email))
            ModelState.AddModelError(nameof(vm.Email), "Zaposlenik s tim emailom već postoji.");

        if (!ModelState.IsValid)
        {
            vm.Sektori = await DohvatiSektoreAsync(vm.SektorId);
            return View(vm);
        }

        var zaposlenik = new Zaposlenik
        {
            Ime = vm.Ime,
            Prezime = vm.Prezime,
            Email = vm.Email,
            Radnomjesto = vm.Radnomjesto,
            SektorId = vm.SektorId!.Value,
            Aktivan = true
        };

        _context.Add(zaposlenik);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // EDIT: GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var z = await _context.Zaposlenici.FindAsync(id);
        if (z == null) return NotFound();

        var vm = new ZaposlenikFormViewModel
        {
            Id = z.Id,
            Ime = z.Ime,
            Prezime = z.Prezime,
            Email = z.Email,
            Radnomjesto = z.Radnomjesto,
            SektorId = z.SektorId,
            Aktivan = z.Aktivan,
            Sektori = await DohvatiSektoreAsync(z.SektorId)
        };
        return View(vm);
    }

    // EDIT: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ZaposlenikFormViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        var zaposlenik = await _context.Zaposlenici.FindAsync(id);
        if (zaposlenik == null) return NotFound();

        if (await _context.Zaposlenici.AnyAsync(z => z.Email == vm.Email && z.Id != id))
            ModelState.AddModelError(nameof(vm.Email), "Zaposlenik s tim emailom već postoji.");

        if (!vm.Aktivan && zaposlenik.Aktivan &&
            await _context.Zaduzenja.AnyAsync(a => a.ZaposlenikId == id && a.DatumRazduzenja == null))
        {
            ModelState.AddModelError(nameof(vm.Aktivan),
                "Zaposlenik ima zaduženu opremu pa se ne može deaktivirati. Prvo razduži opremu.");
        }

        if (!ModelState.IsValid)
        {
            vm.Sektori = await DohvatiSektoreAsync(vm.SektorId);
            return View(vm);
        }

        zaposlenik.Ime = vm.Ime;
        zaposlenik.Prezime = vm.Prezime;
        zaposlenik.Email = vm.Email;
        zaposlenik.Radnomjesto = vm.Radnomjesto;
        zaposlenik.SektorId = vm.SektorId!.Value;
        zaposlenik.Aktivan = vm.Aktivan;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // READ: detalji
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vm = await _context.Zaposlenici
            .Where(z => z.Id == id)
            .Select(z => new ZaposlenikDetailsViewModel
            {
                Id = z.Id,
                PunoIme = z.Ime + " " + z.Prezime,
                Email = z.Email,
                Radnomjesto = z.Radnomjesto,
                Sektor = z.Sektor!.Naziv,
                Aktivan = z.Aktivan,
                Zaduzenja = z.Zaduzenja
                    .OrderByDescending(a => a.DatumZaduzenja)
                    .Select(a => new ZaduzenjeRedakViewModel
                    {
                        Id = a.Id,
                        OpremaId = a.OpremaId,
                        Oprema = a.Oprema!.Proizvodac + " " + a.Oprema.Model,
                        InventarniBroj = a.Oprema.InventarniBroj,
                        DatumZaduzenja = a.DatumZaduzenja,
                        DatumRazduzenja = a.DatumRazduzenja,
                        Napomena = a.Napomena
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }
    // DELETE: GET (potvrda)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var vm = await _context.Zaposlenici
            .Where(z => z.Id == id)
            .Select(z => new ZaposlenikDetailsViewModel
            {
                Id = z.Id,
                PunoIme = z.Ime + " " + z.Prezime,
                Email = z.Email,
                Radnomjesto = z.Radnomjesto,
                Sektor = z.Sektor!.Naziv,
                Aktivan = z.Aktivan,
                BrojZaduzenjaUkupno = z.Zaduzenja.Count(),
                BrojAktivnihZaduzenja = z.Zaduzenja.Count(a => a.DatumRazduzenja == null)
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }

    // DELETE: POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var zaposlenik = await _context.Zaposlenici.FindAsync(id);
        if (zaposlenik == null) return NotFound();

        if (await _context.Zaduzenja.AnyAsync(a => a.ZaposlenikId == id))
        {
            TempData["Greska"] = "Zaposlenik ima povijest zaduženja i ne može se obrisati. Označi ga kao neaktivnog.";
            return RedirectToAction(nameof(Index));
        }

        _context.Zaposlenici.Remove(zaposlenik);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}