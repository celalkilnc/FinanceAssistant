using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinanceAssistant.API.Data;
using FinanceAssistant.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinanceAssistant.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegularIncomesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public RegularIncomesController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/RegularIncomes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegularIncome>>> GetRegularIncomes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.RegularIncomes
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.DayOfMonth)
                .ToListAsync();
        }

        // GET: api/RegularIncomes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegularIncome>> GetRegularIncome(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var regularIncome = await _context.RegularIncomes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (regularIncome == null)
            {
                return NotFound();
            }

            return regularIncome;
        }

        // GET: api/RegularIncomes/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<RegularIncome>>> GetActiveRegularIncomes()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.RegularIncomes
                .Where(r => r.UserId == userId && r.IsActive)
                .OrderBy(r => r.NextPaymentDate)
                .ToListAsync();
        }

        // POST: api/RegularIncomes
        [HttpPost]
        public async Task<ActionResult<RegularIncome>> CreateRegularIncome(RegularIncome regularIncome)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Validate DayOfMonth
            if (regularIncome.DayOfMonth < 1 || regularIncome.DayOfMonth > 31)
            {
                return BadRequest("Day of month must be between 1 and 31.");
            }

            regularIncome.UserId = userId;
            regularIncome.CreatedAt = DateTime.UtcNow;
            regularIncome.IsActive = true;

            // Calculate next payment date
            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, Math.Min(regularIncome.DayOfMonth, DateTime.DaysInMonth(today.Year, today.Month)));
            regularIncome.NextPaymentDate = thisMonth < today ? thisMonth.AddMonths(1) : thisMonth;

            _context.RegularIncomes.Add(regularIncome);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRegularIncome), new { id = regularIncome.Id }, regularIncome);
        }

        // PUT: api/RegularIncomes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRegularIncome(int id, RegularIncome regularIncome)
        {
            if (id != regularIncome.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingIncome = await _context.RegularIncomes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (existingIncome == null)
            {
                return NotFound();
            }

            // Validate DayOfMonth
            if (regularIncome.DayOfMonth < 1 || regularIncome.DayOfMonth > 31)
            {
                return BadRequest("Day of month must be between 1 and 31.");
            }

            existingIncome.Name = regularIncome.Name;
            existingIncome.Amount = regularIncome.Amount;
            existingIncome.DayOfMonth = regularIncome.DayOfMonth;
            existingIncome.Description = regularIncome.Description;
            existingIncome.IsActive = regularIncome.IsActive;
            existingIncome.UpdatedAt = DateTime.UtcNow;

            // Update next payment date if day of month changed
            if (existingIncome.DayOfMonth != regularIncome.DayOfMonth)
            {
                var today = DateTime.UtcNow.Date;
                var thisMonth = new DateTime(today.Year, today.Month, Math.Min(regularIncome.DayOfMonth, DateTime.DaysInMonth(today.Year, today.Month)));
                existingIncome.NextPaymentDate = thisMonth < today ? thisMonth.AddMonths(1) : thisMonth;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegularIncomeExists(id))
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

        // DELETE: api/RegularIncomes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegularIncome(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var regularIncome = await _context.RegularIncomes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (regularIncome == null)
            {
                return NotFound();
            }

            _context.RegularIncomes.Remove(regularIncome);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/RegularIncomes/5/toggle-active
        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var regularIncome = await _context.RegularIncomes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (regularIncome == null)
            {
                return NotFound();
            }

            regularIncome.IsActive = !regularIncome.IsActive;
            regularIncome.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegularIncomeExists(int id)
        {
            return _context.RegularIncomes.Any(e => e.Id == id);
        }
    }
} 