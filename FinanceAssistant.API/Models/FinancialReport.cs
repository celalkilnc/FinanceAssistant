using System;
using System.Collections.Generic;

namespace FinanceAssistant.API.Models
{
    public class FinancialReport
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }  // Monthly, Annual, Custom
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public string Period { get; set; }  // Örn: "2024-01" veya "2024"
        public Dictionary<string, decimal> CategorySummary { get; set; }  // Kategori bazlı özet
        public Dictionary<string, decimal> MonthlyTrend { get; set; }    // Aylık trend
        public string Notes { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }

    public class FinancialReportDetail
    {
        public int Id { get; set; }
        public int FinancialReportId { get; set; }
        public virtual FinancialReport Report { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal PercentageOfTotal { get; set; }
        public Dictionary<string, decimal> DailyDistribution { get; set; }
    }
} 