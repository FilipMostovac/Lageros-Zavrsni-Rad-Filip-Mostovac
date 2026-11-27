using System.Diagnostics;
using System.Globalization;
using Lageros.Data;
using Lageros.Models;
using Lageros.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var hr = new CultureInfo("hr-HR");
        var sada = DateTime.Now;
        var korisnik = await _userManager.GetUserAsync(User);

        // Jedan SQL GROUP BY upit: broj komada po vrsti i statusu
        var stanja = await _context.Oprema
            .GroupBy(o => new { o.Vrsta, o.Status })
            .Select(g => new { g.Key.Vrsta, g.Key.Status, Broj = g.Count() })
            .ToListAsync();

        int Broj(VrstaOpreme? vrsta, StatusOpreme status) =>
            stanja.Where(x => (vrsta == null || x.Vrsta == vrsta) && x.Status == status)
                  .Sum(x => x.Broj);

        var zaposlenikaSOpremom = await _context.Zaduzenja
            .Where(z => z.DatumRazduzenja == null)
            .Select(z => z.ZaposlenikId)
            .Distinct()
            .CountAsync();

        var pocetak = new DateTime(sada.Year, sada.Month, 1).AddMonths(-11);

        var zaduzenja = await _context.Zaduzenja
            .Where(z => z.DatumZaduzenja >= pocetak)
            .Select(z => z.DatumZaduzenja)
            .ToListAsync();

        var razduzenja = await _context.Zaduzenja
            .Where(z => z.DatumRazduzenja != null && z.DatumRazduzenja >= pocetak)
            .Select(z => z.DatumRazduzenja!.Value)
            .ToListAsync();

        var mjeseci = Enumerable.Range(0, 12).Select(i =>
        {
            var m = pocetak.AddMonths(i);
            return new MjesecStatistikaViewModel
            {
                Oznaka = hr.DateTimeFormat.GetAbbreviatedMonthName(m.Month),
                Zaduzeno = zaduzenja.Count(d => d.Year == m.Year && d.Month == m.Month),
                Vraceno = razduzenja.Count(d => d.Year == m.Year && d.Month == m.Month)
            };
        }).ToList();

        var najveci = mjeseci.Max(m => Math.Max(m.Zaduzeno, m.Vraceno));
        var vrhOsi = Math.Max(4, (int)Math.Ceiling(najveci / 4.0) * 4);

        var model = new DashboardViewModel
        {
            Datum = sada.ToString("dddd, d. MMMM yyyy.", hr).ToUpper(hr),
            Pozdrav = (sada.Hour < 10 ? "Dobro jutro" : sada.Hour < 18 ? "Dobar dan" : "Dobra večer")
                      + ", " + (korisnik?.Ime ?? ""),

            UUpotrebi = Broj(null, StatusOpreme.Zaduzeno),
            NaSkladistu = Broj(null, StatusOpreme.NaSkladistu),
            NaServisu = Broj(null, StatusOpreme.NaServisu),
            Otpisano = Broj(null, StatusOpreme.Otpisano),
            ZaposlenikaSOpremom = zaposlenikaSOpremom,

            PoVrstama = Enum.GetValues<VrstaOpreme>().Select(v => new VrstaStatistikaViewModel
            {
                Naslov = v switch
                {
                    VrstaOpreme.Laptop => "Laptopi",
                    VrstaOpreme.Monitor => "Monitori",
                    _ => "Dodatna oprema"
                },
                UUpotrebi = Broj(v, StatusOpreme.Zaduzeno),
                NaSkladistu = Broj(v, StatusOpreme.NaSkladistu),
                NaServisu = Broj(v, StatusOpreme.NaServisu)
            }).ToList(),

            PoMjesecima = mjeseci,
            VrhOsi = vrhOsi
        };

        model.Ukupno = model.UUpotrebi + model.NaSkladistu + model.NaServisu;

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}