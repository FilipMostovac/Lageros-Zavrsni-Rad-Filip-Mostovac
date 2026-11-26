using Lageros.Models;
using Microsoft.AspNetCore.Identity;

namespace Lageros.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var email = config["PocetniAdmin:Email"];
        var lozinka = config["PocetniAdmin:Lozinka"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(lozinka))
            return;

        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Ime = config["PocetniAdmin:Ime"] ?? "Glavni",
            Prezime = config["PocetniAdmin:Prezime"] ?? "Administrator"
        };

        await userManager.CreateAsync(admin, lozinka);
    }
}