using System.ComponentModel.DataAnnotations;

namespace Lageros.Models.ViewModels;

public class PrijavaViewModel
{
    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Neispravan email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; } = string.Empty;

    [Display(Name = "Zapamti me")]
    public bool ZapamtiMe { get; set; }
}