using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SUUO_DZ3.Data;
using SUUO_DZ3.Models;

namespace SUUO_DZ3.Controllers;

[ApiController]
[Route("api/kuhar")]
public class KuharController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public KuharController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/kuhar
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var kuhari = await _context.Kuhari.ToListAsync();
        return Ok(kuhari);
    }

    // GET: api/kuhar/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var kuhar = await _context.Kuhari
            .FirstOrDefaultAsync(m => m.IdKuhar == id);

        if (kuhar == null)
            return NotFound();

        return Ok(kuhar);
    }

    // POST: api/kuhar
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Kuhar kuhar)
    {
        if (kuhar == null)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        kuhar.IdKuhar = Guid.NewGuid();
        _context.Kuhari.Add(kuhar);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = kuhar.IdKuhar }, kuhar);
    }

    // PUT: api/kuhar/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Kuhar kuhar)
    {
        if (kuhar == null || id != kuhar.IdKuhar)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _context.Entry(kuhar).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await KuharExistsAsync(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    // DELETE: api/kuhar/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var kuhar = await _context.Kuhari.FindAsync(id);
        if (kuhar == null)
            return NotFound();

        _context.Kuhari.Remove(kuhar);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict("Ne može se obrisati kuhar jer je povezan s narudžbama ili stavkama.");
        }

        return NoContent();
    }

    private async Task<bool> KuharExistsAsync(Guid id)
    {
        return await _context.Kuhari.AnyAsync(e => e.IdKuhar == id);
    }
}
