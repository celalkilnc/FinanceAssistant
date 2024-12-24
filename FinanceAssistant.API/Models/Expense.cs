using System;

namespace FinanceAssistant.API.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string PaymentMethod { get; set; }

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 