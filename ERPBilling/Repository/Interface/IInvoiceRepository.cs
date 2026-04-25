using ERPBilling.Models;

namespace ERPBilling.Repository.Interface
{
    public interface IInvoiceRepository
    {
        // Fetch all invoices with Customer + Payments (for list page)
        Task<IEnumerable<Invoice>> GetAllAsync();

        // Fetch invoices filtered by status — Paid / Unpaid / Partial
        Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status);

        // Fetch one invoice with Customer + InvoiceItems + Products + Payments (for Details page)
        Task<Invoice?> GetByIdWithDetailsAsync(int id);

        // Fetch just the invoice header (for payment validation)
        Task<Invoice?> GetByIdAsync(int id);

        // Insert a new invoice (EF Core cascades to save InvoiceItems too)
        Task AddAsync(Invoice invoice);

        // Update invoice — mainly used to update Status after payment
        Task UpdateAsync(Invoice invoice);

        // Get the count of all invoices — used for invoice number generation
        Task<int> GetCountAsync();

        // Check if an invoice number already exists (prevent duplicates)
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber);
    }
}
