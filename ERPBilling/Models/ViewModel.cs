using System.ComponentModel.DataAnnotations;

namespace ERPBilling.Models
{
    /// <summary>
    /// ViewModel for Invoice Create form
    /// Used to carry form data + line items from the view to the controller
    /// Not mapped to any DB table — only used in memory
    /// </summary>
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);

        // List of line items added dynamically on the form via JavaScript
        public List<InvoiceItemViewModel> Items { get; set; } = new();

        // Totals — calculated by JS on the frontend, sent as hidden fields
        public decimal Subtotal { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
    }

    /// <summary>
    /// Represents one product row in the invoice create form
    /// Bound from form fields like Items[0].ProductId, Items[0].Quantity etc.
    /// </summary>
    public class InvoiceItemViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }

    /// <summary>
    /// ViewModel for the Dashboard page
    /// Carries summary statistics and recent invoice list
    /// Not mapped to any DB table — built in HomeController
    /// </summary>
    public class DashboardViewModel
    {
        // Summary numbers shown in the 4 stat cards
        [Display(Name = "Total Revenue (₹)")]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Total Pending (₹)")]
        public decimal TotalPending { get; set; }

        public int TotalPaidInvoices { get; set; }
        public int TotalUnpaidInvoices { get; set; }
        public int TotalPartialInvoices { get; set; }
        public int TotalInvoices { get; set; }

        // Recent 5 invoices shown in dashboard table
        public List<Invoice> RecentInvoices { get; set; } = new();
    }
}