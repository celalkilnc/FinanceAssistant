using System;

namespace FinanceAssistant.API.Models
{
    public class Installment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal MonthlyAmount { get; set; }
        public int TotalInstallments { get; set; } // Toplam taksit sayısı
        public int CurrentInstallmentNumber { get; set; } // Şu anki taksit sayısı
        public DateTime StartDate { get; set; }
        public DateTime LastPaymentDate { get; set; }
        public bool IsCompleted { get; set; }
        
        public int CardId { get; set; }
        public virtual Card Card { get; set; }
    }
} 