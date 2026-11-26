using Lageros.Models;
using Lageros.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lageros.Controllers;

public class RacunController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    public RacunController(SignInManager<ApplicationUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    public IActionResult Prijava(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new PrijavaViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Prijava(PrijavaViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var rezultat = await _signInManager.PasswordSignInAsync(
            vm.Email, vm.Lozinka, vm.ZapamtiMe, lockoutOnFailure: true);

        if (rezultat.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, rezultat.IsLockedOut
            ? "Račun je privremeno zaključan zbog previše pokušaja."
            : "Neispravan email ili lozinka.");
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Odjava()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Prijava));
    }
}