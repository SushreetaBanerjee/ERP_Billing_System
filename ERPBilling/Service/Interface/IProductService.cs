using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);

        // Returns (success, errorMessage) — checks if product is in any invoice
        Task<(bool Success, string? Error)> DeleteProductAsync(int id);

        // Returns unit price and tax percent as anonymous object for JS
        Task<Product?> GetProductForJsonAsync(int id);
    }
}
