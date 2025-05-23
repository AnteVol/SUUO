using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;

namespace SUUO_DZ3.Controllers;

[ApiController]
[Route("api/konobar")]
public class KonobarController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public KonobarController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/konobar
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var konobari = await _context.Konobari.ToListAsync();
        return Ok(konobari);
    }

    // GET: api/konobar/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var konobar = await _context.Konobari
            .Include(k => k.Narudzbe)
            .FirstOrDefaultAsync(m => m.IdKonobar == id);

        if (konobar == null)
            return NotFound();

        return Ok(konobar);
    }

    // POST: api/konobar
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Konobar konobar)
    {
        if (konobar == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        konobar.IdKonobar = Guid.NewGuid();
        _context.Konobari.Add(konobar);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = konobar.IdKonobar }, konobar);
    }

    // PUT: api/konobar/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Konobar konobar)
    {
        if (konobar == null || id != konobar.IdKonobar)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(konobar).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await KonobarExistsAsync(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/konobar/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var konobar = await _context.Konobari.FindAsync(id);
        if (konobar == null)
            return NotFound();

        _context.Konobari.Remove(konobar);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Ne može se obrisati konobar jer je povezan s narudžbama.");
        }

        return NoContent();
    }

    private async Task<bool> KonobarExistsAsync(Guid id)
    {
        return await _context.Konobari.AnyAsync(e => e.IdKonobar == id);
    }
}
