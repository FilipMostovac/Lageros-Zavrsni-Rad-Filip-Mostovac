using Microsoft.AspNetCore.Identity;

namespace Lageros.Models;

public class ApplicationUser : IdentityUser
{
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
}