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
    public class InvoicesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public InvoicesController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Invoices
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // GET: api/Invoices/unpaid
        [HttpGet("unpaid")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetUnpaidInvoices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Invoices
                .Where(i => i.UserId == userId && !i.IsPaid && i.DueDate >= DateTime.Now)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        // GET: api/Invoices/recurring
        [HttpGet("recurring")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetRecurringInvoices()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Invoices
                .Where(i => i.UserId == userId && i.IsRecurring)
                .OrderBy(i => i.RecurringDay)
                .ToListAsync();
        }

        // GET: api/Invoices/byType/{type}
        [HttpGet("byType/{type}")]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoicesByType(string type)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.Invoices
                .Where(i => i.UserId == userId && i.InvoiceType == type)
                .OrderByDescending(i => i.DueDate)
                .ToListAsync();
        }

        // POST: api/Invoices
        [HttpPost]
        public async Task<ActionResult<Invoice>> CreateInvoice(Invoice invoice)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            invoice.UserId = userId;
            invoice.CreatedAt = DateTime.UtcNow;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Create notification for the new invoice
            var notification = new Notification
            {
                UserId = userId,
                Title = "New Invoice Added",
                Message = $"A new invoice for {invoice.Title} has been added with due date {invoice.DueDate:d}",
                Type = "Invoice",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ReferenceId = invoice.Id,
                ReferenceType = "Invoice",
                IsImportant = true,
                ScheduledFor = invoice.DueDate.AddDays(-3) // Notify 3 days before due date
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }

        // PUT: api/Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (id != invoice.Id)
            {
                return BadRequest();
            }

            var existingInvoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (existingInvoice == null)
            {
                return NotFound();
            }

            existingInvoice.Title = invoice.Title;
            existingInvoice.Provider = invoice.Provider;
            existingInvoice.Amount = invoice.Amount;
            existingInvoice.DueDate = invoice.DueDate;
            existingInvoice.InvoiceNumber = invoice.InvoiceNumber;
            existingInvoice.InvoiceType = invoice.InvoiceType;
            existingInvoice.Period = invoice.Period;
            existingInvoice.IsRecurring = invoice.IsRecurring;
            existingInvoice.RecurringDay = invoice.RecurringDay;
            existingInvoice.Notes = invoice.Notes;
            existingInvoice.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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

        // PUT: api/Invoices/5/pay
        [HttpPut("{id}/pay")]
        public async Task<IActionResult> PayInvoice(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (invoice == null)
            {
                return NotFound();
            }

            invoice.IsPaid = true;
            invoice.PaymentDate = DateTime.UtcNow;
            invoice.UpdatedAt = DateTime.UtcNow;

            // Create notification for the payment
            var notification = new Notification
            {
                UserId = userId,
                Title = "Invoice Paid",
                Message = $"Payment recorded for {invoice.Title}",
                Type = "Invoice",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                ReferenceId = invoice.Id,
                ReferenceType = "Invoice",
                IsImportant = false
            };

            _context.Notifications.Add(notification);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvoiceExists(id))
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

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
                
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return _context.Invoices.Any(e => e.Id == id && e.UserId == userId);
        }
    }
} 