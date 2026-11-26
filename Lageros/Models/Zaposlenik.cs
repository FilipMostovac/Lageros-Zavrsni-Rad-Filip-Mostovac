using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Lageros.Models;

public class Zaposlenik
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(50)]
    public string Ime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [StringLength(50)]
    public string Prezime { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email je obavezan.")]
    [EmailAddress(ErrorMessage = "Neispravan email.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Radnomjesto { get; set; }

    public bool Aktivan { get; set; } = true;

    // Strani ključ + navigacijsko svojstvo prema sektoru
    [Required(ErrorMessage = "Odaberi sektor.")]
    public int SektorId { get; set; }
    public Sektor? Sektor { get; set; }

    public ICollection<Zaduzenje> Zaduzenja { get; set; } = new List<Zaduzenje>();

    // Izračunato svojstvo, ne sprema se u bazu
    public string PunoIme => $"{Ime} {Prezime}";
}