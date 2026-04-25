using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Repository
{
    public class InvoiceRepository:IInvoiceRepository
    {
        private readonly ERPBillingdbContext _context;

        public InvoiceRepository(ERPBillingdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        
        public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Payments)
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        
        public async Task<Invoice?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)  
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        
        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.Payments)   
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        
        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        
        public async Task<int> GetCountAsync()
        {
            return await _context.Invoices.CountAsync();
        }

        
        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
        {
            return await _context.Invoices
                .AnyAsync(i => i.InvoiceNumber == invoiceNumber);
        }
    }
}
