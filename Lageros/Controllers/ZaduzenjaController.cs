using Lageros.Data;
using Lageros.Models;
using Lageros.Models.ViewModels;
using Lageros.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class ZaduzenjaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ZaduzenjeService _servis;

    public ZaduzenjaController(ApplicationDbContext context, ZaduzenjeService servis)
    {
        _context = context;
        _servis = servis;
    }

    // Povijest svih zaduženja
    public async Task<IActionResult> Index(string? trazi, VrstaOpreme? vrsta, bool samoAktivna = false)
    {
        var upit = _context.Zaduzenja.AsQueryable();

        if (samoAktivna) upit = upit.Where(z => z.DatumRazduzenja == null);
        if (vrsta.HasValue) upit = upit.Where(z => z.Oprema!.Vrsta == vrsta);

        if (!string.IsNullOrWhiteSpace(trazi))
        {
            trazi = trazi.Trim();
            upit = upit.Where(z => z.Zaposlenik!.Ime.Contains(trazi)
                                || z.Zaposlenik.Prezime.Contains(trazi)
                                || z.Oprema!.Proizvodac.Contains(trazi)
                                || z.Oprema.Model.Contains(trazi)
                                || z.Oprema.InventarniBroj.Contains(trazi));
        }

        var model = new ZaduzenjeIndexViewModel
        {
            Trazi = trazi,
            Vrsta = vrsta,
            SamoAktivna = samoAktivna,
            Zaduzenja = await upit
                .OrderByDescending(z => z.DatumZaduzenja).ThenByDescending(z => z.Id)
                .Select(z => new ZaduzenjeListaRedakViewModel
                {
                    Id = z.Id,
                    OpremaId = z.OpremaId,
                    ZaposlenikId = z.ZaposlenikId,
                    Oprema = z.Oprema!.Proizvodac + " " + z.Oprema.Model,
                    InventarniBroj = z.Oprema.InventarniBroj,
                    Vrsta = z.Oprema.Vrsta,
                    Zaposlenik = z.Zaposlenik!.Ime + " " + z.Zaposlenik.Prezime,
                    DatumZaduzenja = z.DatumZaduzenja,
                    DatumRazduzenja = z.DatumRazduzenja,
                    Napomena = z.Napomena,
                    Administrator = z.RazduzioKorisnik ?? z.ZaduzioKorisnik
                })
                .ToListAsync()
        };

        return View(model);
    }

    // ZADUŽI: GET
    public async Task<IActionResult> Zaduzi(int? opremaId)
    {
        if (opremaId == null) return NotFound();

        var oprema = await _context.Oprema.FindAsync(opremaId);
        if (oprema == null) return NotFound();

        if (oprema.Status != StatusOpreme.NaSkladistu)
        {
            TempData["Greska"] = "Oprema nije na skladištu pa se ne može zadužiti.";
            return RedirectToAction("Index", "Oprema");
        }

        return View(new ZaduziViewModel
        {
            OpremaId = oprema.Id,
            OpremaOpis = $"{oprema.Proizvodac} {oprema.Model} ({oprema.InventarniBroj})",
            Zaposlenici = await DohvatiAktivneAsync()
        });
    }

    // ZADUŽI: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Zaduzi(ZaduziViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Zaposlenici = await DohvatiAktivneAsync(vm.ZaposlenikId);
            return View(vm);
        }

        var rezultat = await _servis.ZaduziAsync(
            vm.OpremaId, vm.ZaposlenikId!.Value, vm.Datum, vm.Napomena, User.Identity?.Name);

        if (!rezultat.Uspjeh)
        {
            ModelState.AddModelError(string.Empty, rezultat.Poruka!);
            vm.Zaposlenici = await DohvatiAktivneAsync(vm.ZaposlenikId);
            return View(vm);
        }

        TempData["Poruka"] = "Oprema je zadužena.";
        return RedirectToAction("Index", "Oprema");
    }

    // RAZDUŽI: GET
    public async Task<IActionResult> Razduzi(int? opremaId)
    {
        if (opremaId == null) return NotFound();

        var z = await _context.Zaduzenja
            .Include(x => x.Oprema)
            .Include(x => x.Zaposlenik)
            .FirstOrDefaultAsync(x => x.OpremaId == opremaId && x.DatumRazduzenja == null);

        if (z == null)
        {
            TempData["Greska"] = "Oprema nema aktivno zaduženje.";
            return RedirectToAction("Index", "Oprema");
        }

        return View(new RazduziViewModel
        {
            ZaduzenjeId = z.Id,
            OpremaOpis = $"{z.Oprema!.Proizvodac} {z.Oprema.Model} ({z.Oprema.InventarniBroj})",
            ZaposlenikIme = z.Zaposlenik!.PunoIme,
            DatumZaduzenja = z.DatumZaduzenja
        });
    }

    // RAZDUŽI: POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Razduzi(RazduziViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var rezultat = await _servis.RazduziAsync(
            vm.ZaduzenjeId, vm.Datum, vm.Napomena, User.Identity?.Name);

        if (!rezultat.Uspjeh)
        {
            ModelState.AddModelError(string.Empty, rezultat.Poruka!);
            return View(vm);
        }

        TempData["Poruka"] = "Oprema je razdužena.";
        return RedirectToAction("Index", "Oprema");
    }

    private async Task<SelectList> DohvatiAktivneAsync(int? odabrani = null)
    {
        var lista = await _context.Zaposlenici
            .Where(z => z.Aktivan)
            .OrderBy(z => z.Prezime).ThenBy(z => z.Ime)
            .Select(z => new { z.Id, Naziv = z.Ime + " " + z.Prezime })
            .ToListAsync();
        return new SelectList(lista, "Id", "Naziv", odabrani);
    }
}