using System;

namespace FinanceAssistant.API.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Provider { get; set; }  // Fatura sağlayıcı (Elektrik, Su, Doğalgaz şirketi vb.)
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsPaid { get; set; }
        public string InvoiceNumber { get; set; }  // Fatura numarası
        public string InvoiceType { get; set; }    // Elektrik, Su, Doğalgaz, İnternet vb.
        public string Period { get; set; }         // Fatura dönemi (Örn: Ocak 2024)
        public bool IsRecurring { get; set; }      // Düzenli fatura mı?
        public int? RecurringDay { get; set; }     // Düzenli faturanın hangi gün geldiği
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // User relationship
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
} 