using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lageros.Models.ViewModels;

public class ZaduziViewModel
{
    public int OpremaId { get; set; }
    public string OpremaOpis { get; set; } = string.Empty;

    [Required(ErrorMessage = "Odaberi zaposlenika.")]
    [Display(Name = "Zaposlenik")]
    public int? ZaposlenikId { get; set; }

    [Required(ErrorMessage = "Datum je obavezan.")]
    [DataType(DataType.Date)]
    [Display(Name = "Datum zaduženja")]
    public DateTime Datum { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Napomena { get; set; }

    public SelectList? Zaposlenici { get; set; }
}

public class RazduziViewModel
{
    public int ZaduzenjeId { get; set; }
    public string OpremaOpis { get; set; } = string.Empty;
    public string ZaposlenikIme { get; set; } = string.Empty;
    public DateTime DatumZaduzenja { get; set; }

    [Required(ErrorMessage = "Datum je obavezan.")]
    [DataType(DataType.Date)]
    [Display(Name = "Datum razduženja")]
    public DateTime Datum { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Napomena { get; set; }
}

public class ZaduzenjeListaRedakViewModel
{
    public int Id { get; set; }
    public int ZaposlenikId { get; set; }
    public int OpremaId { get; set; }
    public string Oprema { get; set; } = string.Empty;
    public string InventarniBroj { get; set; } = string.Empty;
    public VrstaOpreme Vrsta { get; set; }
    public string Zaposlenik { get; set; } = string.Empty;
    public DateTime DatumZaduzenja { get; set; }
    public DateTime? DatumRazduzenja { get; set; }
    public string? Napomena { get; set; }
    public string? Administrator { get; set; }
    public bool Aktivno => DatumRazduzenja == null;
}

public class ZaduzenjeIndexViewModel
{
    public string? Trazi { get; set; }
    public VrstaOpreme? Vrsta { get; set; }
    public bool SamoAktivna { get; set; }
    public List<ZaduzenjeListaRedakViewModel> Zaduzenja { get; set; } = new();
}