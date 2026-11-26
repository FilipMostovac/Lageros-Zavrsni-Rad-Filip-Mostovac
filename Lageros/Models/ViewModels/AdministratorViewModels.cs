using System.ComponentModel.DataAnnotations;

namespace Lageros.Models.ViewModels;

public class AdministratorRedakViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Ime { get; set; } = string.Empty;
    public string Prezime { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool JeTrenutni { get; set; }
    public string Inicijali =>
        $"{(Ime.Length > 0 ? Ime[0] : '?')}{(Prezime.Length > 0 ? Prezime[0] : '?')}";
}

public class NoviAdministratorViewModel
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(50)]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [StringLength(50)]
    public string Prezime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Neispravan email.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Lozinka je obavezna.")]
    [DataType(DataType.Password)]
    public string Lozinka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Potvrdi lozinku.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Lozinka), ErrorMessage = "Lozinke se ne podudaraju.")]
    [Display(Name = "Potvrda lozinke")]
    public string PotvrdaLozinke { get; set; } = string.Empty;
}