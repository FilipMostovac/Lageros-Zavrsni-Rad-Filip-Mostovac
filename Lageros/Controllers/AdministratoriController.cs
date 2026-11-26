using Lageros.Models;
using Lageros.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Controllers;

public class AdministratoriController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdministratoriController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var trenutniId = _userManager.GetUserId(User);

        var admini = await _userManager.Users
            .OrderBy(u => u.Prezime).ThenBy(u => u.Ime)
            .Select(u => new AdministratorRedakViewModel
            {
                Id = u.Id,
                Ime = u.Ime,
                Prezime = u.Prezime,
                Email = u.Email ?? "",
                JeTrenutni = u.Id == trenutniId
            })
            .ToListAsync();

        return View(admini);
    }

    public IActionResult Create() => View(new NoviAdministratorViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NoviAdministratorViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var korisnik = new ApplicationUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            EmailConfirmed = true,
            Ime = vm.Ime,
            Prezime = vm.Prezime
        };

        var rezultat = await _userManager.CreateAsync(korisnik, vm.Lozinka);

        if (!rezultat.Succeeded)
        {
            foreach (var greska in rezultat.Errors)
                ModelState.AddModelError(string.Empty, greska.Description);
            return View(vm);
        }

        TempData["Poruka"] = $"Administrator {vm.Email} je dodan.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var korisnik = await _userManager.FindByIdAsync(id);
        if (korisnik == null) return NotFound();

        if (korisnik.Id == _userManager.GetUserId(User))
        {
            TempData["Greska"] = "Ne možeš obrisati vlastiti račun.";
            return RedirectToAction(nameof(Index));
        }

        if (await _userManager.Users.CountAsync() <= 1)
        {
            TempData["Greska"] = "Ne može se obrisati zadnji administrator.";
            return RedirectToAction(nameof(Index));
        }

        await _userManager.DeleteAsync(korisnik);
        TempData["Poruka"] = $"Administrator {korisnik.Email} je obrisan.";
        return RedirectToAction(nameof(Index));
    }
}