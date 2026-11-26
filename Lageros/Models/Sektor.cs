using System.ComponentModel.DataAnnotations;

namespace Lageros.Models;

public class Sektor
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100)]
    public string Naziv { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Opis { get; set; }

    public ICollection<Zaposlenik> Zaposlenici { get; set; } = new List<Zaposlenik>();
}