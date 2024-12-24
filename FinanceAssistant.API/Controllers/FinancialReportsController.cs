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
    public class FinancialReportsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public FinancialReportsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/FinancialReports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialReport>>> GetReports()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return await _context.FinancialReports
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.GeneratedAt)
                .ToListAsync();
        }

        // GET: api/FinancialReports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FinancialReport>> GetReport(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var report = await _context.FinancialReports
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null)
            {
                return NotFound();
            }

            return report;
        }

        // GET: api/FinancialReports/monthly/2024-01
        [HttpGet("monthly/{period}")]
        public async Task<ActionResult<FinancialReport>> GetMonthlyReport(string period)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Parse period (format: YYYY-MM)
            if (!DateTime.TryParse($"{period}-01", out DateTime startDate))
            {
                return BadRequest("Invalid period format. Use YYYY-MM");
            }

            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Check if report already exists
            var existingReport = await _context.FinancialReports
                .FirstOrDefaultAsync(r => r.UserId == userId && 
                                        r.Type == "Monthly" && 
                                        r.Period == period);

            if (existingReport != null)
            {
                return existingReport;
            }

            // Calculate report data
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId && 
                           i.Date >= startDate && 
                           i.Date <= endDate)
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId && 
                           e.Date >= startDate && 
                           e.Date <= endDate)
                .ToListAsync();

            var bills = await _context.Bills
                .Where(b => b.UserId == userId && 
                           b.DueDate >= startDate && 
                           b.DueDate <= endDate)
                .ToListAsync();

            var invoices = await _context.Invoices
                .Where(i => i.UserId == userId && 
                           i.DueDate >= startDate && 
                           i.DueDate <= endDate)
                .ToListAsync();

            // Calculate totals
            decimal totalIncome = incomes.Sum(i => i.Amount);
            decimal totalExpense = expenses.Sum(e => e.Amount) + 
                                 bills.Where(b => b.IsPaid).Sum(b => b.Amount) +
                                 invoices.Where(i => i.IsPaid).Sum(i => i.Amount);

            // Create category summary
            var categorySummary = new Dictionary<string, decimal>();
            foreach (var expense in expenses)
            {
                if (!categorySummary.ContainsKey(expense.Category))
                    categorySummary[expense.Category] = 0;
                categorySummary[expense.Category] += expense.Amount;
            }

            // Create monthly trend
            var monthlyTrend = new Dictionary<string, decimal>
            {
                { "Income", totalIncome },
                { "Expense", totalExpense },
                { "Balance", totalIncome - totalExpense }
            };

            // Create new report
            var report = new FinancialReport
            {
                Title = $"Monthly Report - {period}",
                Type = "Monthly",
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                Period = period,
                CategorySummary = categorySummary,
                MonthlyTrend = monthlyTrend,
                GeneratedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            // Create report details
            var reportDetails = new List<FinancialReportDetail>();
            foreach (var category in categorySummary.Keys)
            {
                var categoryExpenses = expenses.Where(e => e.Category == category).ToList();
                var detail = new FinancialReportDetail
                {
                    FinancialReportId = report.Id,
                    Category = category,
                    Amount = categorySummary[category],
                    TransactionCount = categoryExpenses.Count(),
                    AverageAmount = categoryExpenses.Count() > 0 ? 
                        categoryExpenses.Average(e => e.Amount) : 0,
                    PercentageOfTotal = totalExpense > 0 ? 
                        (categorySummary[category] / totalExpense) * 100 : 0,
                    DailyDistribution = categoryExpenses
                        .GroupBy(e => e.Date.Date)
                        .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Sum(e => e.Amount))
                };
                reportDetails.Add(detail);
            }

            _context.FinancialReportDetails.AddRange(reportDetails);
            await _context.SaveChangesAsync();

            return report;
        }

        // GET: api/FinancialReports/annual/2024
        [HttpGet("annual/{year}")]
        public async Task<ActionResult<FinancialReport>> GetAnnualReport(int year)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var startDate = new DateTime(year, 1, 1);
            var endDate = startDate.AddYears(1).AddDays(-1);

            // Check if report already exists
            var existingReport = await _context.FinancialReports
                .FirstOrDefaultAsync(r => r.UserId == userId && 
                                        r.Type == "Annual" && 
                                        r.Period == year.ToString());

            if (existingReport != null)
            {
                return existingReport;
            }

            // Calculate report data
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId && 
                           i.Date.Year == year)
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId && 
                           e.Date.Year == year)
                .ToListAsync();

            var bills = await _context.Bills
                .Where(b => b.UserId == userId && 
                           b.DueDate.Year == year)
                .ToListAsync();

            var invoices = await _context.Invoices
                .Where(i => i.UserId == userId && 
                           i.DueDate.Year == year)
                .ToListAsync();

            // Calculate totals
            decimal totalIncome = incomes.Sum(i => i.Amount);
            decimal totalExpense = expenses.Sum(e => e.Amount) + 
                                 bills.Where(b => b.IsPaid).Sum(b => b.Amount) +
                                 invoices.Where(i => i.IsPaid).Sum(i => i.Amount);

            // Create category summary
            var categorySummary = new Dictionary<string, decimal>();
            foreach (var expense in expenses)
            {
                if (!categorySummary.ContainsKey(expense.Category))
                    categorySummary[expense.Category] = 0;
                categorySummary[expense.Category] += expense.Amount;
            }

            // Create monthly trend
            var monthlyTrend = expenses
                .GroupBy(e => e.Date.ToString("yyyy-MM"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(e => e.Amount)
                );

            // Create new report
            var report = new FinancialReport
            {
                Title = $"Annual Report - {year}",
                Type = "Annual",
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                Period = year.ToString(),
                CategorySummary = categorySummary,
                MonthlyTrend = monthlyTrend,
                GeneratedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            // Create report details
            var reportDetails = new List<FinancialReportDetail>();
            foreach (var category in categorySummary.Keys)
            {
                var categoryExpenses = expenses.Where(e => e.Category == category).ToList();
                var detail = new FinancialReportDetail
                {
                    FinancialReportId = report.Id,
                    Category = category,
                    Amount = categorySummary[category],
                    TransactionCount = categoryExpenses.Count(),
                    AverageAmount = categoryExpenses.Count() > 0 ? 
                        categoryExpenses.Average(e => e.Amount) : 0,
                    PercentageOfTotal = totalExpense > 0 ? 
                        (categorySummary[category] / totalExpense) * 100 : 0,
                    DailyDistribution = categoryExpenses
                        .GroupBy(e => e.Date.Date)
                        .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Sum(e => e.Amount))
                };
                reportDetails.Add(detail);
            }

            _context.FinancialReportDetails.AddRange(reportDetails);
            await _context.SaveChangesAsync();

            return report;
        }

        // GET: api/FinancialReports/custom
        [HttpGet("custom")]
        public async Task<ActionResult<FinancialReport>> GetCustomReport(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Validate dates
            if (startDate >= endDate)
            {
                return BadRequest("Start date must be before end date");
            }

            // Calculate report data
            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId && 
                           i.Date >= startDate && 
                           i.Date <= endDate)
                .ToListAsync();

            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId && 
                           e.Date >= startDate && 
                           e.Date <= endDate)
                .ToListAsync();

            var bills = await _context.Bills
                .Where(b => b.UserId == userId && 
                           b.DueDate >= startDate && 
                           b.DueDate <= endDate)
                .ToListAsync();

            var invoices = await _context.Invoices
                .Where(i => i.UserId == userId && 
                           i.DueDate >= startDate && 
                           i.DueDate <= endDate)
                .ToListAsync();

            // Calculate totals
            decimal totalIncome = incomes.Sum(i => i.Amount);
            decimal totalExpense = expenses.Sum(e => e.Amount) + 
                                 bills.Where(b => b.IsPaid).Sum(b => b.Amount) +
                                 invoices.Where(i => i.IsPaid).Sum(i => i.Amount);

            // Create category summary
            var categorySummary = new Dictionary<string, decimal>();
            foreach (var expense in expenses)
            {
                if (!categorySummary.ContainsKey(expense.Category))
                    categorySummary[expense.Category] = 0;
                categorySummary[expense.Category] += expense.Amount;
            }

            // Create monthly trend
            var monthlyTrend = expenses
                .GroupBy(e => e.Date.ToString("yyyy-MM"))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(e => e.Amount)
                );

            // Create new report
            var report = new FinancialReport
            {
                Title = $"Custom Report ({startDate:d} - {endDate:d})",
                Type = "Custom",
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                Period = $"{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}",
                CategorySummary = categorySummary,
                MonthlyTrend = monthlyTrend,
                GeneratedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.FinancialReports.Add(report);
            await _context.SaveChangesAsync();

            // Create report details
            var reportDetails = new List<FinancialReportDetail>();
            foreach (var category in categorySummary.Keys)
            {
                var categoryExpenses = expenses.Where(e => e.Category == category).ToList();
                var detail = new FinancialReportDetail
                {
                    FinancialReportId = report.Id,
                    Category = category,
                    Amount = categorySummary[category],
                    TransactionCount = categoryExpenses.Count(),
                    AverageAmount = categoryExpenses.Count() > 0 ? 
                        categoryExpenses.Average(e => e.Amount) : 0,
                    PercentageOfTotal = totalExpense > 0 ? 
                        (categorySummary[category] / totalExpense) * 100 : 0,
                    DailyDistribution = categoryExpenses
                        .GroupBy(e => e.Date.Date)
                        .ToDictionary(g => g.Key.ToString("yyyy-MM-dd"), g => g.Sum(e => e.Amount))
                };
                reportDetails.Add(detail);
            }

            _context.FinancialReportDetails.AddRange(reportDetails);
            await _context.SaveChangesAsync();

            return report;
        }

        // DELETE: api/FinancialReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var report = await _context.FinancialReports
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (report == null)
            {
                return NotFound();
            }

            // Delete related details first
            var details = await _context.FinancialReportDetails
                .Where(d => d.FinancialReportId == id)
                .ToListAsync();
            
            _context.FinancialReportDetails.RemoveRange(details);
            _context.FinancialReports.Remove(report);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 