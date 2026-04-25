using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPBilling.Models
{
    /// <summary>
    /// Payment mode — stored as TINYINT in SQL
    /// 0=Cash | 1=BankTransfer | 2=Cheque | 3=UPI | 4=CreditCard
    /// Must match the CHECK constraint in your SQL table
    /// </summary>
    public enum PaymentMode
    {
        Cash = 0,
        BankTransfer = 1,
        Cheque = 2,
        UPI = 3,
        CreditCard = 4
    }

    /// <summary>
    /// Maps to dbo.Payments table in SQL Server
    /// Records each payment made against an invoice
    /// </summary>
    public class Payment
    {
        // Primary Key — maps to Id INT IDENTITY(1,1)
        public int Id { get; set; }

        // Foreign Key → Invoices table
        [Required]
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "Amount paid is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Amount Paid (₹)")]
        public decimal AmountPaid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Today;

        // PaymentMode stored as TINYINT (0-4) in SQL
        [Display(Name = "Payment Mode")]
        public PaymentMode PaymentMode { get; set; } = PaymentMode.Cash;

        [StringLength(300)]
        public string? Notes { get; set; }         // nullable — matches NULL in SQL

        // Navigation property — lets us do payment.Invoice.InvoiceNumber in views
        public Invoice? Invoice { get; set; }
    }
}