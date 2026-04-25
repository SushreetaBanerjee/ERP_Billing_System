using ERPBilling.Models;
using ERPBilling.Repository;
using ERPBilling.Repository.Interface;
using ERPBilling.Service.Interface;

namespace ERPBilling.Services
{
    
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepo.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepo.GetByIdAsync(id);
        }

        public async Task CreateProductAsync(Product product)
        {
            await _productRepo.AddAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepo.UpdateAsync(product);
        }

        /// <summary>
        /// Delete — with business rule:
        /// Cannot delete a product that appears in any InvoiceItem.
        /// Matches ON DELETE NO ACTION constraint in SQL.
        /// </summary>
        public async Task<(bool Success, string? Error)> DeleteProductAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
                return (false, "Product not found.");

            
            bool isUsed = await _productRepo.IsUsedInInvoicesAsync(id);
            if (isUsed)
                return (false, $"Cannot delete '{product.Name}' — it is referenced in existing invoices.");

            await _productRepo.DeleteAsync(id);
            return (true, null);
        }

        
        public async Task<Product?> GetProductForJsonAsync(int id)
        {
            return await _productRepo.GetByIdAsync(id);
        }
    }
}