using System;

namespace FinanceAssistant.API.Models
{
    public class Income
    {
        public int Id { get; set; }
        public string Source { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurrenceInterval { get; set; } // Monthly, Weekly, Yearly etc.

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 