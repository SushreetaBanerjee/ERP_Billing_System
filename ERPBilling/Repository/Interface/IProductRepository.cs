using ERPBilling.Models;

namespace ERPBilling.Repository.Interface
{
    public interface IProductRepository
    {
        // Fetch all products ordered by category then name
        Task<IEnumerable<Product>> GetAllAsync();

        // Fetch one product by ID
        Task<Product?> GetByIdAsync(int id);

        // Fetch product WITH its invoice items (used to check if deletable)
        Task<Product?> GetByIdWithItemsAsync(int id);

        // Insert a new product
        Task AddAsync(Product product);

        // Update an existing product
        Task UpdateAsync(Product product);

        // Delete a product by ID
        Task DeleteAsync(int id);

        // Check if product exists
        Task<bool> ExistsAsync(int id);

        // Check if product is used in any invoice (prevents deletion)
        Task<bool> IsUsedInInvoicesAsync(int id);
    }
}
