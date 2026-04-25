using ERPBilling.Models;
using ERPBilling.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ERPBilling.Controllers
{
   
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }
        public async Task<IActionResult> Index(string? status)
        {
            IEnumerable<Invoice> invoices;
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<InvoiceStatus>(status, out var parsed))
                invoices = await _invoiceService.GetInvoicesByStatusAsync(parsed);
            else
                invoices = await _invoiceService.GetAllInvoicesAsync();
            ViewBag.CurrentStatus = status;
            return View(invoices);
        }
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetInvoiceWithDetailsAsync(id);
            if (invoice == null) return NotFound();
            decimal paid = invoice.Payments.Sum(p => p.AmountPaid);
            decimal balance = invoice.TotalAmount - paid;
            ViewBag.AmountPaid = paid;
            ViewBag.BalanceDue = balance;
            return View(invoice);
        }
        public async Task<IActionResult> Create()
        {
            var (customers, products) = await _invoiceService.GetFormDataAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");
            ViewBag.Products = products.Select(p => new { p.Id, p.Name, p.UnitPrice, p.TaxPercent }).ToList();
            var vm = new InvoiceViewModel
            {
                InvoiceNumber = await _invoiceService.GenerateInvoiceNumberAsync(),
                InvoiceDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(30)
            };
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceViewModel vm)
        {
            if (vm.Items == null || !vm.Items.Any())
                ModelState.AddModelError("", "Please add at least one line item.");
            if (!ModelState.IsValid)
            {
                var (customers, products) = await _invoiceService.GetFormDataAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name");
                ViewBag.Products = products.Select(p => new { p.Id, p.Name, p.UnitPrice, p.TaxPercent }).ToList();
                return View(vm);
            }
            var invoice = await _invoiceService.CreateInvoiceAsync(vm);
            TempData["Success"] = $"Invoice {invoice.InvoiceNumber} created successfully.";
            return RedirectToAction(nameof(Details), new { id = invoice.Id });
        }
    }
}