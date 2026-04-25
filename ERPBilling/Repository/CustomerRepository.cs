using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Repository
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly ERPBillingdbContext _context;

        public CustomerRepository(ERPBillingdbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                            .OrderBy(c=>c.Name)
                            .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer?> GetByIdWithInvoicesAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Invoices)   
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }
    }
}
