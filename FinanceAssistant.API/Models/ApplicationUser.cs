using Microsoft.AspNetCore.Identity;

namespace FinanceAssistant.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigation property
        public virtual UserSettings Settings { get; set; }
    }
} 