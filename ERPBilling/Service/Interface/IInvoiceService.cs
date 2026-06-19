using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
        Task<Invoice?> GetInvoiceWithDetailsAsync(int id);

        // Generates next invoice number e.g. INV-0006
        Task<string> GenerateInvoiceNumberAsync();

        // Creates invoice from ViewModel — calculates totals, saves to DB
        Task<Invoice> CreateInvoiceAsync(InvoiceViewModel vm);

        // Fetches customers and products for the Create form dropdowns
        Task<(IEnumerable<Customer> Customers, IEnumerable<Product> Products)> GetFormDataAsync();

       
    }
}
