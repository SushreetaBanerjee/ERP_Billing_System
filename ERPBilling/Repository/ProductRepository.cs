using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Repository
{
    public class ProductRepository:IProductRepository
    {
        private readonly ERPBillingdbContext _context;
        
        public ProductRepository(ERPBillingdbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                        .OrderBy(p=>p.Category)
                        .ThenBy(p => p.Name)
                        .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        
        public async Task<Product?> GetByIdWithItemsAsync(int id)
        {
            return await _context.Products
                .Include(p => p.InvoiceItems)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

       
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        
        public async Task<bool> IsUsedInInvoicesAsync(int id)
        {
            return await _context.InvoiceItems.AnyAsync(ii => ii.ProductId == id);
        }
    }
}
