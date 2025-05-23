using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;

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
    public async Task<IActionResult> GetAll()
    {
        var stavke = await _context.StavkeNarudzbe //           .Include(s => s.Narudzba)
            .ToListAsync();

        return Ok(stavke);
    }

    // GET: api/stavkanarudzbe/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var stavka = await _context.StavkeNarudzbe
//            .Include(s => s.Narudzba)
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
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        stavka.StavkaNarudzbeId = Guid.NewGuid();
        _context.StavkeNarudzbe.Add(stavka);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = stavka.StavkaNarudzbeId }, stavka);
    }

    // PUT: api/stavkanarudzbe/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] StavkaNarudzbe stavka)
    {
        if (stavka == null || id != stavka.StavkaNarudzbeId)
            return BadRequest();

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
        }
        catch (DbUpdateException)
        {
            return Conflict("Ne može se obrisati stavka jer je povezana s drugim entitetima.");
        }

        return NoContent();
    }

    // GET: api/stavkanarudzbe/bynarudzba/{narudzbaId}
    [HttpGet("bynarudzba/{narudzbaId}")]
    public async Task<IActionResult> GetByNarudzba(Guid narudzbaId)
    {
        var stavke = await _context.StavkeNarudzbe
//            .Include(s => s.Narudzba)
            .Where(s => s.NarudzbaId == narudzbaId)
            .ToListAsync();

        return Ok(stavke);
    }

    // GET: api/stavkanarudzbe/bystatus/{status}
    [HttpGet("bystatus/{status}")]
    public async Task<IActionResult> GetByStatus(StatusStavke status)
    {
        var stavke = await _context.StavkeNarudzbe
//            .Include(s => s.Narudzba)
            .Where(s => s.Status == status)
            .ToListAsync();

        return Ok(stavke);
    }

    // GET: api/stavkanarudzbe/akcijskeponude
    [HttpGet("akcijskeponude")]
    public async Task<IActionResult> GetAkcijskePonude()
    {
        var akcijskeStavke = await _context.StavkeNarudzbe
//            .Include(s => s.Narudzba)
            .Where(s => s.AkcijskaPonuda)
            .ToListAsync();

        return Ok(akcijskeStavke);
    }

    // GET: api/stavkanarudzbe/statistics
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = new
        {
            UkupnoStavki = await _context.StavkeNarudzbe.CountAsync(),
            AkcijskeStavke = await _context.StavkeNarudzbe.CountAsync(s => s.AkcijskaPonuda),
            UkupnaVrijednost = await _context.StavkeNarudzbe.SumAsync(s => s.Kolicina * s.Cijena),
            ProsjekCijena = await _context.StavkeNarudzbe.AverageAsync(s => s.Cijena),
            StavkePoStatusu = await _context.StavkeNarudzbe
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync()
        };

        return Ok(stats);
    }

    private async Task<bool> StavkaNarudzbeExistsAsync(Guid id)
    {
        return await _context.StavkeNarudzbe.AnyAsync(e => e.StavkaNarudzbeId == id);
    }
}
