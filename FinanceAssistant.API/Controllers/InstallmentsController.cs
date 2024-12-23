using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceAssistant.API.Data;
using FinanceAssistant.API.Models;

namespace FinanceAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstallmentsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InstallmentsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Installments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Installment>>> GetInstallments()
        {
            return await _context.Installments
                .Include(i => i.Card)
                .ToListAsync();
        }

        // GET: api/Installments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Installment>> GetInstallment(int id)
        {
            var installment = await _context.Installments
                .Include(i => i.Card)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (installment == null)
            {
                return NotFound();
            }

            return installment;
        }

        // GET: api/Installments/card/5
        [HttpGet("card/{cardId}")]
        public async Task<ActionResult<IEnumerable<Installment>>> GetInstallmentsByCard(int cardId)
        {
            return await _context.Installments
                .Where(i => i.CardId == cardId)
                .OrderByDescending(i => i.StartDate)
                .ToListAsync();
        }

        // GET: api/Installments/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<Installment>>> GetActiveInstallments()
        {
            return await _context.Installments
                .Include(i => i.Card)
                .Where(i => !i.IsCompleted)
                .OrderBy(i => i.LastPaymentDate)
                .ToListAsync();
        }

        // POST: api/Installments
        [HttpPost]
        public async Task<ActionResult<Installment>> PostInstallment(Installment installment)
        {
            _context.Installments.Add(installment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInstallment), new { id = installment.Id }, installment);
        }

        // PUT: api/Installments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstallment(int id, Installment installment)
        {
            if (id != installment.Id)
            {
                return BadRequest();
            }

            _context.Entry(installment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstallmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PUT: api/Installments/5/increment
        [HttpPut("{id}/increment")]
        public async Task<IActionResult> IncrementInstallment(int id)
        {
            var installment = await _context.Installments.FindAsync(id);

            if (installment == null)
            {
                return NotFound();
            }

            installment.CurrentInstallmentNumber++;
            if (installment.CurrentInstallmentNumber >= installment.TotalInstallments)
            {
                installment.IsCompleted = true;
            }

            installment.LastPaymentDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InstallmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Installments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstallment(int id)
        {
            var installment = await _context.Installments.FindAsync(id);
            if (installment == null)
            {
                return NotFound();
            }

            _context.Installments.Remove(installment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InstallmentExists(int id)
        {
            return _context.Installments.Any(e => e.Id == id);
        }
    }
} 