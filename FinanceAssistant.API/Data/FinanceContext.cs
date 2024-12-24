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
        public DbSet<RegularIncome> RegularIncomes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }
        public DbSet<FinancialReportDetail> FinancialReportDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL için tablo isimlerini küçük harfe çevir
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Tablo adını küçük harfe çevir
                entity.SetTableName(entity.GetTableName().ToLower());

                // Tüm kolonları küçük harfe çevir
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }

                // Tüm primary key constraint isimlerini küçük harfe çevir
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToLower());
                }

                // Tüm foreign key constraint isimlerini küçük harfe çevir
                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToLower());
                }

                // Tüm index isimlerini küçük harfe çevir
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToLower());
                }
            }

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

            modelBuilder.Entity<RegularIncome>()
                .Property(r => r.Amount)
                .HasPrecision(18, 2);

            // RegularIncome configuration
            modelBuilder.Entity<RegularIncome>()
                .Property(r => r.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<RegularIncome>()
                .Property(r => r.IsActive)
                .HasDefaultValue(true);

            modelBuilder.Entity<RegularIncome>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<RegularIncome>()
                .Property(r => r.DayOfMonth)
                .IsRequired();

            // User - RegularIncome relationship
            modelBuilder.Entity<RegularIncome>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // PostgreSQL syntax

            modelBuilder.Entity<UserSettings>()
                .Property(s => s.IsTwoFactorEnabled)
                .HasDefaultValue(false);

            // Invoice configuration
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Amount)
                .HasPrecision(18, 2);

            // FinancialReport configuration
            modelBuilder.Entity<FinancialReport>()
                .Property(f => f.TotalIncome)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReport>()
                .Property(f => f.TotalExpense)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReport>()
                .Property(f => f.Balance)
                .HasPrecision(18, 2);

            // FinancialReportDetail configuration
            modelBuilder.Entity<FinancialReportDetail>()
                .Property(f => f.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReportDetail>()
                .Property(f => f.AverageAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<FinancialReportDetail>()
                .Property(f => f.PercentageOfTotal)
                .HasPrecision(5, 2);
        }
    }
} 