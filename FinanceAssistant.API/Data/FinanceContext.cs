using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinanceAssistant.API.Models;

namespace FinanceAssistant.API.Data
{
    public class FinanceContext : IdentityDbContext<ApplicationUser>
    {
        public FinanceContext(DbContextOptions<FinanceContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Para alanları için hassasiyet ayarları
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Bill>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Income>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Installment>()
                .Property(i => i.MonthlyAmount)
                .HasPrecision(18, 2);

            // Kart ve Taksit ilişkisi
            modelBuilder.Entity<Card>()
                .HasMany(c => c.Installments)
                .WithOne(i => i.Card)
                .HasForeignKey(i => i.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kart numarası için maksimum uzunluk
            modelBuilder.Entity<Card>()
                .Property(c => c.LastFourDigits)
                .HasMaxLength(4);

            // Kullanıcı alanları için zorunluluk ayarları
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Email)
                .IsRequired();

            // UserSettings ilişkisi
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserSettings için default değerler
            modelBuilder.Entity<UserSettings>()
                .Property(s => s.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<UserSettings>()
                .Property(s => s.IsTwoFactorEnabled)
                .HasDefaultValue(false);
        }
    }
} 