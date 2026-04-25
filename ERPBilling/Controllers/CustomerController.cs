using ERPBilling.Models;
using ERPBilling.Service.Interface;
using Microsoft.AspNetCore.Mvc;


namespace ERPBilling.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return View(customers);
        }
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetCustomerWithInvoicesAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        public IActionResult Create() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid) return View(customer);
            await _customerService.CreateCustomerAsync(customer);
            TempData["Success"] = $"Customer '{customer.Name}' created successfully.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id) return BadRequest();
            if (!ModelState.IsValid) return View(customer);
            await _customerService.UpdateCustomerAsync(customer);
            TempData["Success"] = $"Customer '{customer.Name}' updated.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetCustomerWithInvoicesAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (success, error) = await _customerService.DeleteCustomerAsync(id);
            if (!success) TempData["Error"] = error;
            else TempData["Success"] = "Customer deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}