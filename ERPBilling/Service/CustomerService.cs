using ERPBilling.Models;
using ERPBilling.Repository;
using ERPBilling.Repository.Interface;
using ERPBilling.Service.Interface;

namespace ERPBilling.Services
{
   
    public class CustomerService : ICustomerService
    {
        // Repository injected via constructor — loose coupling via interface
        private readonly ICustomerRepository _customerRepo;

        public CustomerService(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepo.GetAllAsync();
        }

       
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepo.GetByIdAsync(id);
        }

        
        public async Task<Customer?> GetCustomerWithInvoicesAsync(int id)
        {
            return await _customerRepo.GetByIdWithInvoicesAsync(id);
        }

        
        public async Task CreateCustomerAsync(Customer customer)
        {
            await _customerRepo.AddAsync(customer);
        }

       
        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _customerRepo.UpdateAsync(customer);
        }

       
        public async Task<(bool Success, string? Error)> DeleteCustomerAsync(int id)
        {
            // Fetch customer with invoices to check the business rule
            var customer = await _customerRepo.GetByIdWithInvoicesAsync(id);

            if (customer == null)
                return (false, "Customer not found.");

            
            // Cannot delete if any invoice is linked to this customer
            if (customer.Invoices.Any())
                return (false, $"Cannot delete '{customer.Name}' — they have {customer.Invoices.Count} existing invoice(s).");

            await _customerRepo.DeleteAsync(id);
            return (true, null);
        }
    }
}