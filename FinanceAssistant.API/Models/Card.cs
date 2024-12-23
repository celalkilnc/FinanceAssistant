using System.Collections.Generic;

namespace FinanceAssistant.API.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; } // Kartın özel ismi
        public string LastFourDigits { get; set; } // Kart numarasının son 4 hanesi
        public string BankName { get; set; }
        public virtual ICollection<Installment> Installments { get; set; }
    }
} 