using System;

namespace FinanceAssistant.API.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public string BillType { get; set; } // Elektrik, Su, Doğalgaz vb.
        public DateTime? PaymentDate { get; set; }
    }
} 