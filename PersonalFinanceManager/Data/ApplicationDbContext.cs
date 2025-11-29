using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=finance_manager;Username=postgres;Password=1");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(a => a.Balance)
                  .HasPrecision(18, 2);
            entity.Property(a => a.AccountType)
                  .HasConversion<string>()
                  .HasMaxLength(20);
            entity.Property(a => a.Color)
                  .HasMaxLength(7)
                  .HasDefaultValue("#007ACC");
            entity.Property(a => a.CreatedDate)
                  .HasColumnType("timestamp without time zone")
                  .HasDefaultValueSql("NOW()");
        });
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Amount)
                  .HasPrecision(18, 2);
            entity.Property(t => t.Description)
                  .HasMaxLength(500);
            entity.Property(t => t.TransactionType)
                  .HasConversion<string>()
                  .HasMaxLength(10);
            entity.Property(t => t.Date)
                  .HasColumnType("timestamp without time zone")
                  .HasDefaultValueSql("NOW()");
            entity.HasOne(t => t.Account)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(t => t.AccountId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Transactions)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(c => c.CategoryType)
                  .HasConversion<string>()
                  .HasMaxLength(10);
            entity.Property(c => c.Color)
                  .HasMaxLength(7)
                  .HasDefaultValue("#808080");

            entity.HasOne(c => c.ParentCategory)
                  .WithMany(c => c.SubCategories)
                  .HasForeignKey(c => c.ParentCategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
        
    }

}