using System.ComponentModel.DataAnnotations;

namespace Lageros.Models;

public class Oprema
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Inventarni broj je obavezan.")]
    [StringLength(20)]
    [Display(Name = "Inventarni broj")]
    public string InventarniBroj { get; set; } = string.Empty;

    [Display(Name = "Vrsta")]
    public VrstaOpreme Vrsta { get; set; }

    [Required(ErrorMessage = "Proizvođač je obavezan.")]
    [StringLength(50)]
    [Display(Name = "Proizvođač")]
    public string Proizvodac { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model je obavezan.")]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Serijski broj")]
    public string? SerijskiBroj { get; set; }

    [StringLength(50)]
    public string? Podvrsta { get; set; }   // npr. Miš, Docking stanica

    public StatusOpreme Status { get; set; } = StatusOpreme.NaSkladistu;

    [DataType(DataType.Date)]
    [Display(Name = "Datum nabave")]
    public DateTime? DatumNabave { get; set; }

    [StringLength(500)]
    public string? Napomena { get; set; }

    public ICollection<Zaduzenje> Zaduzenja { get; set; } = new List<Zaduzenje>();
}