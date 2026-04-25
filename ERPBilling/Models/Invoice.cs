using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPBilling.Models
{
    /// <summary>
    /// Invoice status — stored as TINYINT in SQL
    /// 0 = Unpaid | 1 = Paid | 2 = Partial
    /// Must match the CHECK constraint in your SQL table
    /// </summary>
    public enum InvoiceStatus
    {
        Unpaid = 0,
        Paid = 1,
        Partial = 2
    }

    /// <summary>
    /// Maps to dbo.Invoices table in SQL Server
    /// </summary>
    public class Invoice
    {
        // Primary Key — maps to Id INT IDENTITY(1,1)
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        // Foreign Key — maps to CustomerId INT in SQL
        // EF Core uses this to JOIN with Customers table
        [Required(ErrorMessage = "Please select a customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        // Status stored as TINYINT in SQL (0/1/2)
        // EF Core automatically converts enum ↔ int
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount (₹)")]
        public decimal TotalAmount { get; set; }

        // ── Navigation Properties ──────────────────────────────
        // Customer navigation — lets us do invoice.Customer.Name in views
        public Customer? Customer { get; set; }

        // One invoice has many line items
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        // One invoice can have many payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    /// <summary>
    /// Maps to dbo.InvoiceItems table in SQL Server
    /// Represents one product row inside an invoice
    /// </summary>
    public class InvoiceItem
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key → Invoices table
        public int InvoiceId { get; set; }

        // Foreign Key → Products table
        [Required(ErrorMessage = "Please select a product")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Price captured at time of invoicing
        // (may differ if product price changes later)
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price (₹)")]
        public decimal UnitPrice { get; set; }

        // Tax amount = UnitPrice × Quantity × TaxPercent / 100
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tax Amount (₹)")]
        public decimal TaxAmount { get; set; }

        // Line total = (UnitPrice × Quantity) + TaxAmount
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Line Total (₹)")]
        public decimal LineTotal { get; set; }

        // ── Navigation Properties ──────────────────────────────
        public Invoice? Invoice { get; set; }
        public Product? Product { get; set; }
    }
}