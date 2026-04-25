using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Repository
{
    public class DashboardRepository:IDashboardRepository
    {
        private readonly ERPBillingdbContext _context;

        public DashboardRepository(ERPBillingdbContext context)
        {
            _context = context;
        }


        // SELECT SUM(AmountPaid) FROM Payments
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Payments
                .SumAsync(p => (decimal?)p.AmountPaid) ?? 0;
        }

        // SELECT COUNT(*) FROM Invoices WHERE Status = @status
        public async Task<int> GetInvoiceCountByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices
                .CountAsync(i => i.Status == status);
        }

        // SELECT TOP 5 Invoices.*, Customers.Name
        // FROM Invoices JOIN Customers ON ...
        // ORDER BY InvoiceDate DESC
        public async Task<IEnumerable<Invoice>> GetRecentInvoicesAsync(int count = 5)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .OrderByDescending(i => i.InvoiceDate)
                .Take(count)
                .ToListAsync();
        }

        // Calculates pending amount for all open invoices
        // pending = TotalAmount - (sum of all payments made)
        public async Task<decimal> GetTotalPendingAsync()
        {
            var openInvoices = await _context.Invoices
                .Where(i => i.Status != InvoiceStatus.Paid)
                .Include(i => i.Payments)
                .ToListAsync();

            return openInvoices
                .Sum(i => i.TotalAmount - i.Payments.Sum(p => p.AmountPaid));
        }
    }
}
