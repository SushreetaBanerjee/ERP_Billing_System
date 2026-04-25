using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERPBilling.Models
{
    public class Product
    {
        // Primary Key — maps to Id INT IDENTITY(1,1)
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; }      // nullable — matches NULL in SQL

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be 0 or more")]
        [Column(TypeName = "decimal(18,2)")]        // matches DECIMAL(18,2) in SQL
        [Display(Name = "Unit Price (₹)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Tax must be between 0 and 100")]
        [Column(TypeName = "decimal(5,2)")]         // matches DECIMAL(5,2) in SQL
        [Display(Name = "Tax %")]
        public decimal TaxPercent { get; set; }

        // Navigation property — one product can appear in many invoice items
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
