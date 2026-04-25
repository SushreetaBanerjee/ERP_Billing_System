using ERPBilling.Models;
using ERPBilling.Service.Interface;
using Microsoft.AspNetCore.Mvc;


namespace ERPBilling.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        public async Task<IActionResult> Create(int invoiceId)
        {
            var (invoice, totalPaid, balanceDue) = await _paymentService.GetPaymentFormDataAsync(invoiceId);
            if (invoice == null) return NotFound();
            if (balanceDue <= 0)
            {
                TempData["Error"] = "This invoice is already fully paid.";
                return RedirectToAction("Details", "Invoice", new { id = invoiceId });
            }
            ViewBag.Invoice = invoice;
            ViewBag.TotalPaid = totalPaid;
            ViewBag.BalanceDue = balanceDue;
            return View(new Payment { InvoiceId = invoiceId, AmountPaid = balanceDue, PaymentDate = DateTime.Today });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                var (invoice, totalPaid, balanceDue) = await _paymentService.GetPaymentFormDataAsync(payment.InvoiceId);
                ViewBag.Invoice = invoice;
                ViewBag.TotalPaid = totalPaid;
                ViewBag.BalanceDue = balanceDue;
                return View(payment);
            }
            var (success, error) = await _paymentService.RecordPaymentAsync(payment);
            if (!success)
            {
                ModelState.AddModelError("AmountPaid", error!);
                var (invoice, totalPaid, balanceDue) = await _paymentService.GetPaymentFormDataAsync(payment.InvoiceId);
                ViewBag.Invoice = invoice;
                ViewBag.TotalPaid = totalPaid;
                ViewBag.BalanceDue = balanceDue;
                return View(payment);
            }
            TempData["Success"] = $"Payment of ₹{payment.AmountPaid:N2} recorded successfully.";
            return RedirectToAction("Details", "Invoice", new { id = payment.InvoiceId });
        }
    }
}