using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerWithInvoicesAsync(int id);
        Task CreateCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);

        // Returns (success: bool, errorMessage: string?)
        // Business rule checked here: cannot delete if customer has invoices
        Task<(bool Success, string? Error)> DeleteCustomerAsync(int id);
    }
}
