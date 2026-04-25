using ERPBilling.Models;
using ERPBilling.Repository;
using ERPBilling.Repository.Interface;
using ERPBilling.Service.Interface;

namespace ERPBilling.Services
{
    /// <summary>
    /// InvoiceService — most complex service in the system.
    /// Handles:
    ///   1. Invoice number auto-generation (INV-0001 format)
    ///   2. Line item calculations (subtotal, tax, line total)
    ///   3. Grand total calculation across all line items
    ///   4. Building the Invoice entity from the ViewModel
    /// All calculation logic lives here — NOT in the controller.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IProductRepository _productRepo;
        private readonly ICustomerRepository _customerRepo;

        public InvoiceService(
            IInvoiceRepository invoiceRepo,
            IProductRepository productRepo,
            ICustomerRepository customerRepo)
        {
            _invoiceRepo = invoiceRepo;
            _productRepo = productRepo;
            _customerRepo = customerRepo;
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _invoiceRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
        {
            return await _invoiceRepo.GetByStatusAsync(status);
        }

        public async Task<Invoice?> GetInvoiceWithDetailsAsync(int id)
        {
            return await _invoiceRepo.GetByIdWithDetailsAsync(id);
        }

        /// <summary>
        /// Generates the next invoice number in INV-XXXX format.
        /// Counts total invoices and increments by 1.
        /// Example: 5 invoices exist → returns "INV-0006"
        /// </summary>
        public async Task<string> GenerateInvoiceNumberAsync()
        {
            int count = await _invoiceRepo.GetCountAsync();
            string candidate = $"INV-{(count + 1):D4}";

            // Edge case: if that number already exists (data inconsistency),
            // keep incrementing until we find a unique one
            while (await _invoiceRepo.InvoiceNumberExistsAsync(candidate))
            {
                count++;
                candidate = $"INV-{(count + 1):D4}";
            }

            return candidate;
        }

        /// <summary>
        /// Returns data needed to populate the Invoice Create form:
        ///   - Customer list for the dropdown
        ///   - Product list passed as JSON to JavaScript
        /// </summary>
        public async Task<(IEnumerable<Customer> Customers, IEnumerable<Product> Products)> GetFormDataAsync()
        {
            var customers = await _customerRepo.GetAllAsync();
            var products = await _productRepo.GetAllAsync();
            return (customers, products);
        }

        /// <summary>
        /// Creates a new invoice from the ViewModel.
        /// Business logic:
        ///   1. For each line item: fetch product tax %, calculate subtotal, tax, line total
        ///   2. Sum all line totals → grand total
        ///   3. Set status = Unpaid (always starts unpaid)
        ///   4. Save invoice + all line items in one DB transaction
        /// </summary>
        public async Task<Invoice> CreateInvoiceAsync(InvoiceViewModel vm)
        {
            // Build the invoice header
            var invoice = new Invoice
            {
                InvoiceNumber = vm.InvoiceNumber,
                CustomerId = vm.CustomerId,
                InvoiceDate = vm.InvoiceDate,
                DueDate = vm.DueDate,
                Status = InvoiceStatus.Unpaid  // always starts as unpaid
            };

            decimal grandTotal = 0;

            // ── Process each line item ────────────────────────────────────
            foreach (var item in vm.Items)
            {
                // Fetch product from DB to get the authoritative tax percent
                // We don't trust the tax value from the form (security)
                var product = await _productRepo.GetByIdAsync(item.ProductId);
                if (product == null) continue;

                // ── Calculation Logic ─────────────────────────────────────
                decimal subtotal = item.UnitPrice * item.Quantity;
                decimal taxAmt = Math.Round(subtotal * product.TaxPercent / 100, 2);
                decimal lineTotal = subtotal + taxAmt;

                grandTotal += lineTotal;

                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxAmount = taxAmt,
                    LineTotal = lineTotal
                });
            }

            invoice.TotalAmount = grandTotal;

            // EF Core saves invoice + all line items in one transaction
            await _invoiceRepo.AddAsync(invoice);

            return invoice;
        }
    }
}