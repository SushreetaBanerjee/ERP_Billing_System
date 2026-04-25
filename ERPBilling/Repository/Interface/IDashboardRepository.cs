using ERPBilling.Models;

namespace ERPBilling.Repository.Interface
{
    public interface IDashboardRepository
    {
        // Sum of all payments received across all invoices
        Task<decimal> GetTotalRevenueAsync();

        // Count of invoices grouped by status
        Task<int> GetInvoiceCountByStatusAsync(InvoiceStatus status);

        // Recent N invoices with Customer info (for dashboard table)
        Task<IEnumerable<Invoice>> GetRecentInvoicesAsync(int count = 5);

        // Total pending amount = sum of (TotalAmount - paid) for open invoices
        Task<decimal> GetTotalPendingAsync();
    }
}
