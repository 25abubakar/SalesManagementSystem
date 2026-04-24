using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SaleAcct> SaleAccts { get; set; }
        public DbSet<SalePlatform> SalePlatforms { get; set; }
        public DbSet<SaleProduct> SaleProducts { get; set; }
        public DbSet<SaleTransactionType> SaleTransactionTypes { get; set; }
        public DbSet<SaleAccount> SaleAccounts { get; set; }
        public DbSet<SaleStatus> SaleStatuses { get; set; }
        public DbSet<SaleChargeType> SaleChargeTypes { get; set; }
        public DbSet<SaleCharge> SaleCharges { get; set; }
        public DbSet<SaleDate> SaleDates { get; set; }
        public DbSet<SaleTransactionDate> SaleTransactionDates { get; set; }
        public DbSet<SaleTransactionDatePivot> SaleTransactionDatePivots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SaleAcct>().ToTable("SaleAcct");
            modelBuilder.Entity<SalePlatform>().ToTable("SalePlatform");
            modelBuilder.Entity<SaleProduct>().ToTable("SaleProduct");
            modelBuilder.Entity<SaleTransactionType>().ToTable("SaleTransactionType");;
            modelBuilder.Entity<SaleStatus>().ToTable("SaleStatus");
            modelBuilder.Entity<SaleChargeType>().ToTable("SaleChargeType");
            modelBuilder.Entity<SaleCharge>().ToTable("SaleCharge");
            modelBuilder.Entity<SaleAcct>().ToTable(tb => tb.HasTrigger("TR_SaleAcct_Audit"));

            modelBuilder.Entity<SaleAcct>()
                .Property(p => p.ProfitAmount)
                .HasComputedColumnSql(
                    "ISNULL(SoldAmount,0) - ISNULL(CostAmount,0) - ISNULL(TotalProCharges,0) - ISNULL(AmazonFee,0) - ISNULL(OtherCharges,0) + ISNULL(TotalRroRebate,0)",
                    stored: true);

            modelBuilder.Entity<SaleAcct>()
                .HasIndex(x => new { x.PlatformId, x.ProductId, x.OrderID })
                .IsUnique()
                .HasFilter("[OrderID] IS NOT NULL");

            modelBuilder.Entity<SaleProduct>()
                .HasOne(p => p.Platform)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.PlatformId);

            modelBuilder.Entity<SaleAccount>()
                .HasOne(a => a.Platform)
                .WithMany()
                .HasForeignKey(a => a.PlatformId);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.Platform)
                .WithMany()
                .HasForeignKey(s => s.PlatformId);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.TransactionType)
                .WithMany()
                .HasForeignKey(s => s.TransactionId);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.FromAccount)
                .WithMany()
                .HasForeignKey(s => s.FromAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.ToAccount)
                .WithMany()
                .HasForeignKey(s => s.ToAccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SaleAcct>()
                .HasOne(s => s.StatusMaster)
                .WithMany()
                .HasForeignKey(s => s.StatusID);

            modelBuilder.Entity<SaleCharge>()
                .HasOne(c => c.Sale)
                .WithMany(s => s.Charges)
                .HasForeignKey(c => c.SaleId);

            modelBuilder.Entity<SaleCharge>()
                .HasOne(c => c.ChargeType)
                .WithMany()
                .HasForeignKey(c => c.ChargeTypeId);

            modelBuilder.Entity<SaleTransactionDate>()
                .HasOne(x => x.SaleDate)
                .WithMany(x => x.SaleTransactionDates)
                .HasForeignKey(x => x.DateLabelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleDate>()
                .HasData(new SaleDate { Id = 1, DateLabel = "TransactionDate" },
                 new SaleDate { Id = 2, DateLabel = "PaymentDate" },
                 new SaleDate { Id = 3, DateLabel = "OrderDate" },
                 new SaleDate { Id = 4, DateLabel = "ProcessDate" },
                 new SaleDate { Id = 5, DateLabel = "SoldDate" }
);

            modelBuilder.Entity<SaleTransactionDatePivot>()
                .HasNoKey()
                .ToView("SaleTransactionDatesPivot");

            modelBuilder.Entity<SaleCharge>()
                .ToTable(tb => tb.HasTrigger("TR_SaleCharge_Audit"));

            modelBuilder.Entity<SaleAccount>()
                .ToTable(tb => tb.HasTrigger("TR_SaleAccount_Audit"));

            modelBuilder.Entity<SaleProduct>()
                .ToTable(tb => tb.HasTrigger("TR_SaleProduct_Audit"));
        }

    }
}