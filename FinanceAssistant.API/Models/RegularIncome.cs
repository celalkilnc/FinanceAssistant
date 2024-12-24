using System;

namespace FinanceAssistant.API.Models
{
    public class RegularIncome
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public int DayOfMonth { get; set; }  // 1-31 arası gün
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 