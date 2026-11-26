using Lageros.Data;
using Lageros.Models;
using Lageros.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class OpremaController : Controller
{
    private readonly ApplicationDbContext _context;

    public OpremaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // READ: lista
    public async Task<IActionResult> Index(string? trazi, VrstaOpreme? vrsta, StatusOpreme? status)
    {
        var upit = _context.Oprema.AsQueryable();

        if (vrsta.HasValue)
            upit = upit.Where(o => o.Vrsta == vrsta);

        if (status.HasValue)
            upit = upit.Where(o => o.Status == status);

        if (!string.IsNullOrWhiteSpace(trazi))
        {
            trazi = trazi.Trim();
            upit = upit.Where(o => o.InventarniBroj.Contains(trazi)
                                || o.Proizvodac.Contains(trazi)
                                || o.Model.Contains(trazi)
                                || (o.SerijskiBroj != null && o.SerijskiBroj.Contains(trazi)));
        }

        var model = new OpremaIndexViewModel
        {
            Trazi = trazi,
            Vrsta = vrsta,
            Status = status,
            Oprema = await upit
                .OrderBy(o => o.InventarniBroj)
                .Select(o => new OpremaRedakViewModel
                {
                    Id = o.Id,
                    InventarniBroj = o.InventarniBroj,
                    Vrsta = o.Vrsta,
                    Proizvodac = o.Proizvodac,
                    Model = o.Model,
                    Podvrsta = o.Podvrsta,
                    SerijskiBroj = o.SerijskiBroj,
                    Status = o.Status,
                    Zaposlenik = o.Zaduzenja
                        .Where(a => a.DatumRazduzenja == null)
                        .Select(a => a.Zaposlenik!.Ime + " " + a.Zaposlenik.Prezime)
                        .FirstOrDefault(),
                    ZaduzenoOd = o.Zaduzenja
                        .Where(a => a.DatumRazduzenja == null)
                        .Select(a => (DateTime?)a.DatumZaduzenja)
                        .FirstOrDefault()
                })
                .ToListAsync()
        };

        return View(model);
    }

    // CREATE: GET
    public IActionResult Create()
    {
        return View(new OpremaFormViewModel());
    }

    // CREATE: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OpremaFormViewModel vm)
    {
        if (vm.Status == StatusOpreme.Zaduzeno)
            ModelState.AddModelError(nameof(vm.Status), "Status Zaduženo postavlja se samo zaduživanjem.");

        if (await _context.Oprema.AnyAsync(o => o.InventarniBroj == vm.InventarniBroj))
            ModelState.AddModelError(nameof(vm.InventarniBroj), "Oprema s tim inventarnim brojem već postoji.");

        if (!ModelState.IsValid) return View(vm);

        var oprema = new Oprema
        {
            InventarniBroj = vm.InventarniBroj,
            Vrsta = vm.Vrsta,
            Proizvodac = vm.Proizvodac,
            Model = vm.Model,
            SerijskiBroj = vm.SerijskiBroj,
            Podvrsta = vm.Podvrsta,
            Status = vm.Status,
            DatumNabave = vm.DatumNabave,
            Napomena = vm.Napomena
        };

        _context.Add(oprema);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // EDIT: GET
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var o = await _context.Oprema.FindAsync(id);
        if (o == null) return NotFound();

        return View(new OpremaFormViewModel
        {
            Id = o.Id,
            InventarniBroj = o.InventarniBroj,
            Vrsta = o.Vrsta,
            Proizvodac = o.Proizvodac,
            Model = o.Model,
            SerijskiBroj = o.SerijskiBroj,
            Podvrsta = o.Podvrsta,
            Status = o.Status,
            DatumNabave = o.DatumNabave,
            Napomena = o.Napomena,
            JeZaduzena = o.Status == StatusOpreme.Zaduzeno
        });
    }

    // EDIT: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OpremaFormViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        var oprema = await _context.Oprema.FindAsync(id);
        if (oprema == null) return NotFound();

        var jeZaduzena = oprema.Status == StatusOpreme.Zaduzeno;

        // Status zaduženo se ne mijenja ručno: ni na njega, ni s njega
        if (jeZaduzena)
            vm.Status = StatusOpreme.Zaduzeno;
        else if (vm.Status == StatusOpreme.Zaduzeno)
            ModelState.AddModelError(nameof(vm.Status), "Status Zaduženo postavlja se samo zaduživanjem.");

        if (await _context.Oprema.AnyAsync(o => o.InventarniBroj == vm.InventarniBroj && o.Id != id))
            ModelState.AddModelError(nameof(vm.InventarniBroj), "Oprema s tim inventarnim brojem već postoji.");

        if (!ModelState.IsValid)
        {
            vm.JeZaduzena = jeZaduzena;
            return View(vm);
        }

        oprema.InventarniBroj = vm.InventarniBroj;
        oprema.Vrsta = vm.Vrsta;
        oprema.Proizvodac = vm.Proizvodac;
        oprema.Model = vm.Model;
        oprema.SerijskiBroj = vm.SerijskiBroj;
        oprema.Podvrsta = vm.Podvrsta;
        oprema.Status = vm.Status;
        oprema.DatumNabave = vm.DatumNabave;
        oprema.Napomena = vm.Napomena;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // DELETE: GET (potvrda)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var o = await _context.Oprema
            .Include(x => x.Zaduzenja)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (o == null) return NotFound();

        return View(o);
    }

    // DELETE: POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var o = await _context.Oprema.FindAsync(id);
        if (o == null) return NotFound();

        if (await _context.Zaduzenja.AnyAsync(a => a.OpremaId == id))
        {
            TempData["Greska"] = "Oprema ima povijest zaduženja i ne može se obrisati. Postavi status Otpisano.";
            return RedirectToAction(nameof(Index));
        }

        _context.Oprema.Remove(o);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    // READ: detalji
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vm = await _context.Oprema
            .Where(o => o.Id == id)
            .Select(o => new OpremaDetailsViewModel
            {
                Id = o.Id,
                InventarniBroj = o.InventarniBroj,
                Vrsta = o.Vrsta,
                Podvrsta = o.Podvrsta,
                Proizvodac = o.Proizvodac,
                Model = o.Model,
                SerijskiBroj = o.SerijskiBroj,
                Status = o.Status,
                DatumNabave = o.DatumNabave,
                Napomena = o.Napomena,
                Povijest = o.Zaduzenja
                    .OrderByDescending(z => z.DatumZaduzenja).ThenByDescending(z => z.Id)
                    .Select(z => new OpremaPovijestRedakViewModel
                    {
                        ZaposlenikId = z.ZaposlenikId,
                        Zaposlenik = z.Zaposlenik!.Ime + " " + z.Zaposlenik.Prezime,
                        DatumZaduzenja = z.DatumZaduzenja,
                        DatumRazduzenja = z.DatumRazduzenja,
                        Napomena = z.Napomena,
                        Administrator = z.RazduzioKorisnik ?? z.ZaduzioKorisnik
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }
}