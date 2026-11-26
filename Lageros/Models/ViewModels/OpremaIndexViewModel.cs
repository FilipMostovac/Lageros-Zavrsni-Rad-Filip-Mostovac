using System.ComponentModel.DataAnnotations;

namespace Lageros.Models.ViewModels;

public class OpremaIndexViewModel
{
    [Display(Name = "Pretraga")]
    public string? Trazi { get; set; }

    public VrstaOpreme? Vrsta { get; set; }
    public StatusOpreme? Status { get; set; }

    public List<OpremaRedakViewModel> Oprema { get; set; } = new();
}