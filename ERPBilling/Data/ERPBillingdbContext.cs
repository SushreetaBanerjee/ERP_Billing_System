using ERPBilling.Models;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Data
{
    public class ERPBillingdbContext:DbContext
    {

        public ERPBillingdbContext(DbContextOptions<ERPBillingdbContext> options)
            : base(options)
        {
        }

        // ── DbSets — one per table ─────────────────────────────────────────
        // Each DbSet<T> maps the C# class T to a SQL table
        // DbSet name = table name in SQL (EF Core matches by convention)

        public DbSet<Customer> Customers { get; set; }   // → dbo.Customers
        public DbSet<Product> Products { get; set; }   // → dbo.Products
        public DbSet<Invoice> Invoices { get; set; }   // → dbo.Invoices
        public DbSet<InvoiceItem> InvoiceItems { get; set; }   // → dbo.InvoiceItems
        public DbSet<Payment> Payments { get; set; }   // → dbo.Payments

        /// <summary>
        /// OnModelCreating — configure relationships and constraints.
        /// This tells EF Core exactly how tables are linked (Foreign Keys).
        /// Must match the CONSTRAINT definitions in your SQL script exactly.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── RELATIONSHIP 1: Invoice → Customer (Many-to-One) ──────────
            // Many invoices belong to one customer
            // FK: Invoices.CustomerId → Customers.Id
            // ON DELETE: NO ACTION (can't delete customer who has invoices)
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)            // Invoice has one Customer
                .WithMany(c => c.Invoices)          // Customer has many Invoices
                .HasForeignKey(i => i.CustomerId)   // FK column in Invoices table
                .OnDelete(DeleteBehavior.Restrict);  // matches ON DELETE NO ACTION in SQL

            // ── RELATIONSHIP 2: InvoiceItem → Invoice (Many-to-One) ───────
            // Many line items belong to one invoice
            // FK: InvoiceItems.InvoiceId → Invoices.Id
            // ON DELETE: CASCADE (deleting invoice deletes its line items)
            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);   // matches ON DELETE CASCADE in SQL

            // ── RELATIONSHIP 3: InvoiceItem → Product (Many-to-One) ───────
            // Many line items reference one product
            // FK: InvoiceItems.ProductId → Products.Id
            // ON DELETE: NO ACTION (can't delete product used in invoices)
            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Product)
                .WithMany(p => p.InvoiceItems)
                .HasForeignKey(ii => ii.ProductId)
                .OnDelete(DeleteBehavior.Restrict);  // matches ON DELETE NO ACTION in SQL

            // ── RELATIONSHIP 4: Payment → Invoice (Many-to-One) ───────────
            // Many payments can be made against one invoice
            // FK: Payments.InvoiceId → Invoices.Id
            // ON DELETE: CASCADE (deleting invoice deletes its payments)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);   // matches ON DELETE CASCADE in SQL

            // ── UNIQUE CONSTRAINTS ────────────────────────────────────────
            // Matches UQ_Customers_Email in SQL
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Matches UQ_Invoices_InvoiceNumber in SQL
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            // ── COLUMN TYPE MAPPINGS ──────────────────────────────────────
            // Tell EF Core to store Status and PaymentMode as integers (TINYINT)
            // This matches your SQL CHECK constraints (0/1/2 and 0/1/2/3/4)
            modelBuilder.Entity<Invoice>()
                .Property(i => i.Status)
                .HasConversion<byte>();               // enum stored as INT/TINYINT

            modelBuilder.Entity<Payment>()
                .Property(p => p.PaymentMode)
                .HasConversion<byte>();               // enum stored as INT/TINYINT
        }
    }
}
