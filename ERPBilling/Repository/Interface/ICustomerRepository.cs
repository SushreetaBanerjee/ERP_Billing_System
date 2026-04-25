using ERPBilling.Models;

namespace ERPBilling.Repository.Interface
{
    public interface ICustomerRepository
    {
        // Fetch all customers ordered by name
        Task<IEnumerable<Customer>> GetAllAsync();

        // Fetch one customer by ID — returns null if not found
        Task<Customer?> GetByIdAsync(int id);

        // Fetch one customer WITH their invoices loaded (for Details page)
        Task<Customer?> GetByIdWithInvoicesAsync(int id);

        // Insert a new customer into the database
        Task AddAsync(Customer customer);

        // Update an existing customer record
        Task UpdateAsync(Customer customer);

        // Delete a customer by ID
        Task DeleteAsync(int id);

        // Check if a customer with this ID exists
        Task<bool> ExistsAsync(int id);
    }
}
