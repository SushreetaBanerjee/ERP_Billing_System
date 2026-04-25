using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Repository
{
    public class PaymentRepository:IPaymentRepository
    {
        private readonly ERPBillingdbContext _context;

        public PaymentRepository(ERPBillingdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

       
        public async Task<decimal> GetTotalPaidByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .SumAsync(p => (decimal?)p.AmountPaid) ?? 0;
        }
    }
}
