using System.ComponentModel.DataAnnotations;

namespace Lageros.Models;

public class Zaduzenje
{
    public int Id { get; set; }

    public int OpremaId { get; set; }
    public Oprema? Oprema { get; set; }

    public int ZaposlenikId { get; set; }
    public Zaposlenik? Zaposlenik { get; set; }

    [DataType(DataType.Date)]
    public DateTime DatumZaduzenja { get; set; } = DateTime.Today;

    // null = zaduženje je još aktivno
    [DataType(DataType.Date)]
    public DateTime? DatumRazduzenja { get; set; }

    [StringLength(500)]
    public string? Napomena { get; set; }

    // Tko je izvršio akciju (email administratora)
    [StringLength(100)]
    public string? ZaduzioKorisnik { get; set; }
    [StringLength(100)]
    public string? RazduzioKorisnik { get; set; }

    public bool Aktivno => DatumRazduzenja == null;
}