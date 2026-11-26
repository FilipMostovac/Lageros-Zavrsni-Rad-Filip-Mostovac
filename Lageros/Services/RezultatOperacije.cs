namespace Lageros.Services;

public class RezultatOperacije
{
    public bool Uspjeh { get; private set; }
    public string? Poruka { get; private set; }

    public static RezultatOperacije Ok() => new() { Uspjeh = true };
    public static RezultatOperacije Greska(string poruka) => new() { Uspjeh = false, Poruka = poruka };
}