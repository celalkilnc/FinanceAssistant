using System;

namespace FinanceAssistant.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }  // Invoice, Bill, Expense, Income, System etc.
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string ActionLink { get; set; }  // İlgili sayfaya yönlendirme linki
        public int? ReferenceId { get; set; }   // İlgili kaydın ID'si (fatura, taksit vb.)
        public string ReferenceType { get; set; } // İlgili kaydın tipi
        public bool IsImportant { get; set; }
        public DateTime? ScheduledFor { get; set; } // İleri tarihli bildirimler için

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 