using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lageros.Models.ViewModels;

public class ZaposlenikFormViewModel
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

    [Required(ErrorMessage = "Odaberi sektor.")]
    [Display(Name = "Sektor")]
    public int? SektorId { get; set; }

    public bool Aktivan { get; set; } = true;

    public SelectList? Sektori { get; set; }
}