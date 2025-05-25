using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;

namespace SUUO_DZ3.Controllers;

[ApiController]
[Route("api/stavkanarudzbe")]
public class StavkaNarudzbeController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StavkaNarudzbeController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/stavkanarudzbe
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? narudzbaId = null)
    {
        IQueryable<StavkaNarudzbe> query = _context.StavkeNarudzbe;
        
        if (narudzbaId.HasValue)
        {
            query = query.Where(s => s.NarudzbaId == narudzbaId.Value);
        }

        var stavke = await query.ToListAsync();
        return Ok(stavke);
    }

    // GET: api/stavkanarudzbe/narudzba/{narudzbaId}
    [HttpGet("narudzba/{narudzbaId}")]
    public async Task<IActionResult> GetByNarudzbaId(Guid narudzbaId)
    {
        var stavke = await _context.StavkeNarudzbe
            .Where(s => s.NarudzbaId == narudzbaId)
            .ToListAsync();

        return Ok(stavke);
    }

    // GET: api/stavkanarudzbe/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var stavka = await _context.StavkeNarudzbe
            .FirstOrDefaultAsync(m => m.StavkaNarudzbeId == id);

        if (stavka == null)
            return NotFound();

        return Ok(stavka);
    }

    // POST: api/stavkanarudzbe
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StavkaNarudzbe stavka)
    {
        if (stavka == null)
            return BadRequest("Stavka ne može biti null");

        Console.WriteLine($"Primljena stavka: {stavka.Naziv}");

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                .ToArray();
            
            Console.WriteLine($"Model validation errors: {string.Join(", ", errors.Select(e => $"{e.Field}: {string.Join(", ", e.Errors)}"))}");
            return BadRequest(ModelState);
        }

        var narudzbaExists = await _context.Narudzbe.AnyAsync(n => n.NarudzbaId == stavka.NarudzbaId);
        if (!narudzbaExists)
        {
            return BadRequest("Narudžba s danim ID-om ne postoji");
        }

        stavka.StavkaNarudzbeId = Guid.NewGuid();
        _context.StavkeNarudzbe.Add(stavka);
        
        try
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"Stavka uspješno kreirana s ID: {stavka.StavkaNarudzbeId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri spremanju stavke: {ex.Message}");
            return StatusCode(500, "Greška pri spremanju stavke u bazu podataka");
        }

        return CreatedAtAction(nameof(GetById), new { id = stavka.StavkaNarudzbeId }, stavka);
    }

    // PUT: api/stavkanarudzbe/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StavkaNarudzbe stavka)
    {
        if (stavka == null || id != stavka.StavkaNarudzbeId)
            return BadRequest("ID ne odgovara ili je stavka null");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(stavka).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await StavkaNarudzbeExistsAsync(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/stavkanarudzbe/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var stavka = await _context.StavkeNarudzbe.FindAsync(id);
        if (stavka == null)
            return NotFound();

        _context.StavkeNarudzbe.Remove(stavka);

        try
        {
            await _context.SaveChangesAsync();
            Console.WriteLine($"Stavka {id} uspješno obrisana");
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"Greška pri brisanju stavke {id}: {ex.Message}");
            return Conflict("Ne može se obrisati stavka jer je povezana s drugim entitetima.");
        }

        return NoContent();
    }

    private async Task<bool> StavkaNarudzbeExistsAsync(Guid id)
    {
        return await _context.StavkeNarudzbe.AnyAsync(e => e.StavkaNarudzbeId == id);
    }
}
