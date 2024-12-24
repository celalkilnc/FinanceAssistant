using System;

namespace FinanceAssistant.API.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string Description { get; set; }
        public string BillType { get; set; }
        public bool IsPaid { get; set; }

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 