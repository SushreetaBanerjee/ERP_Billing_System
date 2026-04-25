using ERPBilling.Models;

namespace ERPBilling.Repository.Interface
{
    public interface IPaymentRepository
    {
        // Fetch all payments for a specific invoice
        Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId);

        // Insert a new payment record
        Task AddAsync(Payment payment);

        // Get total amount paid for a specific invoice
        Task<decimal> GetTotalPaidByInvoiceIdAsync(int invoiceId);
    }
}
