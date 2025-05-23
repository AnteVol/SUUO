using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;
using SUUO_DZ3.Models.Enums;

namespace SUUO_DZ3.Controllers;

[ApiController]
[Route("api/narudzbe")]
public class NarudzbaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public NarudzbaController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/narudzbe
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var narudzbe = await _context.Narudzbe
            .Include(n => n.Konobar)
            .Include(n => n.StavkeNarudzbi)
            .ToListAsync();

        return Ok(narudzbe);
    }

    // GET: api/narudzbe/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var narudzba = await _context.Narudzbe
            .Include(n => n.Konobar)
            .Include(n => n.StavkeNarudzbi)
            .FirstOrDefaultAsync(n => n.NarudzbaId == id);

        if (narudzba == null)
            return NotFound();

        return Ok(narudzba);
    }

    // POST: api/narudzbe
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Narudzba narudzba)
    {
        if (narudzba == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        narudzba.NarudzbaId = Guid.NewGuid();
        _context.Narudzbe.Add(narudzba);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = narudzba.NarudzbaId }, narudzba);
    }

    // PUT: api/narudzbe/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Narudzba narudzba)
    {
        if (narudzba == null || id != narudzba.NarudzbaId)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(narudzba).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await NarudzbaExistsAsync(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/narudzbe/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var narudzba = await _context.Narudzbe.FindAsync(id);
        if (narudzba == null)
            return NotFound();

        _context.Narudzbe.Remove(narudzba);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Ne može se obrisati narudžba jer je povezana s drugim entitetima.");
        }

        return NoContent();
    }

    private async Task<bool> NarudzbaExistsAsync(Guid id)
    {
        return await _context.Narudzbe.AnyAsync(n => n.NarudzbaId == id);
    }
}
