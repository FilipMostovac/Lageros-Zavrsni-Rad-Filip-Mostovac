using Lageros.Data;
using Lageros.Models;
using Microsoft.EntityFrameworkCore;

namespace Lageros.Services;

public class ZaduzenjeService
{
    private readonly ApplicationDbContext _context;

    public ZaduzenjeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RezultatOperacije> ZaduziAsync(
        int opremaId, int zaposlenikId, DateTime datum, string? napomena, string? korisnik)
    {
        var oprema = await _context.Oprema.FindAsync(opremaId);
        if (oprema == null)
            return RezultatOperacije.Greska("Oprema ne postoji.");

        var zaposlenik = await _context.Zaposlenici.FindAsync(zaposlenikId);
        if (zaposlenik == null)
            return RezultatOperacije.Greska("Zaposlenik ne postoji.");

        if (!zaposlenik.Aktivan)
            return RezultatOperacije.Greska("Zaposlenik nije aktivan.");

        if (oprema.Status != StatusOpreme.NaSkladistu)
            return RezultatOperacije.Greska("Oprema nije na skladištu pa se ne može zadužiti.");

        _context.Zaduzenja.Add(new Zaduzenje
        {
            OpremaId = opremaId,
            ZaposlenikId = zaposlenikId,
            DatumZaduzenja = datum.Date,
            Napomena = napomena,
            ZaduzioKorisnik = korisnik
        });
        oprema.Status = StatusOpreme.Zaduzeno;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // npr. dva admina istovremeno zadužuju isti komad: filtrirani indeks to odbija
            return RezultatOperacije.Greska("Oprema je u međuvremenu već zadužena.");
        }

        return RezultatOperacije.Ok();
    }

    public async Task<RezultatOperacije> RazduziAsync(
        int zaduzenjeId, DateTime datum, string? napomena, string? korisnik)
    {
        var zaduzenje = await _context.Zaduzenja
            .Include(z => z.Oprema)
            .FirstOrDefaultAsync(z => z.Id == zaduzenjeId);

        if (zaduzenje == null)
            return RezultatOperacije.Greska("Zaduženje ne postoji.");

        if (zaduzenje.DatumRazduzenja != null)
            return RezultatOperacije.Greska("Zaduženje je već razduženo.");

        if (datum.Date < zaduzenje.DatumZaduzenja.Date)
            return RezultatOperacije.Greska("Datum razduženja ne može biti prije datuma zaduženja.");

        zaduzenje.DatumRazduzenja = datum.Date;
        zaduzenje.RazduzioKorisnik = korisnik;
        if (!string.IsNullOrWhiteSpace(napomena))
            zaduzenje.Napomena = string.IsNullOrWhiteSpace(zaduzenje.Napomena)
                ? napomena
                : zaduzenje.Napomena + " | " + napomena;

        zaduzenje.Oprema!.Status = StatusOpreme.NaSkladistu;

        await _context.SaveChangesAsync();
        return RezultatOperacije.Ok();
    }
}