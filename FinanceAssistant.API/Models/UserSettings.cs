using System;

namespace FinanceAssistant.API.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public virtual ApplicationUser User { get; set; }
    }
} 