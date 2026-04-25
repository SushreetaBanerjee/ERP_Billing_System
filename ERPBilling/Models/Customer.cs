using System.ComponentModel.DataAnnotations;

namespace ERPBilling.Models
{
    public class Customer
    {
        // Primary Key — maps to Id INT IDENTITY(1,1)
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }       // nullable — matches NULL in SQL

        [StringLength(20)]
        [Display(Name = "GST Number")]
        public string? GSTNumber { get; set; }     // nullable — matches NULL in SQL

        // Navigation property — one customer has many invoices
        // EF Core uses this to do JOINs automatically
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
