using System.ComponentModel.DataAnnotations;

namespace Lageros.Models;

public enum VrstaOpreme
{
    [Display(Name = "Laptop")]
    Laptop = 1,

    [Display(Name = "Monitor")]
    Monitor = 2,

    [Display(Name = "Dodatna oprema")]
    DodatnaOprema = 3
}