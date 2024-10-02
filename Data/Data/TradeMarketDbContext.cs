using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Data.Data
{
    public class TradeMarketDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ReceiptDetail> ReceiptsDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Person> Persons { get; set; }

        public TradeMarketDbContext(DbContextOptions<TradeMarketDbContext> options) : base(options)
        {
            
        }

        //fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(p => p.BirthDate)
                    .IsRequired();
            });

            modelBuilder.Entity<Customer>(entity =>
            {

                entity.Property(c => c.DiscountValue)
                    .IsRequired();

                // One-to-One relationship with Person (HasForeignKey on Customer)
                entity.HasOne(c => c.Person)
                    .WithOne()
                    .HasForeignKey<Customer>(c => c.PersonId);

                entity.HasMany(c => c.Receipts)
                    .WithOne(r => r.Customer)
                    .HasForeignKey(r => r.CustomerId);
            });


            modelBuilder.Entity<ReceiptDetail>(entity =>
            {
                entity.Property(rd => rd.ReceiptId).IsRequired();
                entity.Property(rd => rd.ProductId).IsRequired();
                entity.Property(rd => rd.DiscountUnitPrice).IsRequired();
                entity.Property(rd => rd.UnitPrice).IsRequired();
                entity.Property(rd => rd.Quantity).IsRequired();
                entity.HasOne(rd => rd.Receipt)
                    .WithMany(r => r.ReceiptDetails)
                    .HasForeignKey(rd => rd.ReceiptId);
                entity.HasOne(rd => rd.Product)
                    .WithMany(p => p.ReceiptDetails)
                    .HasForeignKey(rd => rd.ProductId);
            });

            modelBuilder.Entity<Receipt>(entity =>
            {
                entity.Property(r => r.OperationDate).IsRequired();
                entity.Property(r => r.IsCheckedOut).IsRequired();
                entity.HasMany(r => r.ReceiptDetails)
                    .WithOne(rd => rd.Receipt)
                    .HasForeignKey(rd => rd.ReceiptId);
                entity.HasOne(r => r.Customer)
                    .WithMany(c => c.Receipts)
                    .HasForeignKey(r => r.CustomerId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ProductCategoryId).IsRequired();
                entity.Property(p => p.ProductName).IsRequired();
                entity.Property(p => p.Price).IsRequired();
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.ProductCategoryId);
                entity.HasMany(p => p.ReceiptDetails)
                    .WithOne(rd => rd.Product)
                    .HasForeignKey(rd => rd.ProductId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(pc => pc.Id);
                entity.Property(pc => pc.CategoryName).IsRequired();
                entity.HasMany(pc => pc.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.ProductCategoryId);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("Market");
                optionsBuilder.UseSqlite("Data Source=marker.db");
                Database.EnsureCreated();
            }
        }

    }
}
