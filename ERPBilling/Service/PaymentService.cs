using ERPBilling.Models;
using ERPBilling.Repository;
using ERPBilling.Repository.Interface;
using ERPBilling.Service.Interface;

namespace ERPBilling.Services
{
    /// <summary>
    /// PaymentService — handles recording payments and updating invoice status.
    /// Core business rules:
    ///   1. Payment cannot exceed balance due
    ///   2. After payment, auto-update invoice status:
    ///      - newPaid >= total  → Paid
    ///      - newPaid > 0       → Partial
    ///      - newPaid == 0      → Unpaid
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IInvoiceRepository _invoiceRepo;

        public PaymentService(
            IPaymentRepository paymentRepo,
            IInvoiceRepository invoiceRepo)
        {
            _paymentRepo = paymentRepo;
            _invoiceRepo = invoiceRepo;
        }

        /// <summary>
        /// Fetches invoice + calculates balance — used to populate the payment form.
        /// Returns: (Invoice, TotalAlreadyPaid, BalanceDue)
        /// </summary>
        public async Task<(Invoice? Invoice, decimal TotalPaid, decimal BalanceDue)> GetPaymentFormDataAsync(int invoiceId)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(invoiceId);

            if (invoice == null)
                return (null, 0, 0);

            decimal totalPaid = await _paymentRepo.GetTotalPaidByInvoiceIdAsync(invoiceId);
            decimal balanceDue = invoice.TotalAmount - totalPaid;

            return (invoice, totalPaid, balanceDue);
        }

        /// <summary>
        /// Records a payment and auto-updates the invoice status.
        /// Returns (true, null) on success or (false, errorMessage) on failure.
        /// </summary>
        public async Task<(bool Success, string? Error)> RecordPaymentAsync(Payment payment)
        {
            var invoice = await _invoiceRepo.GetByIdAsync(payment.InvoiceId);

            if (invoice == null)
                return (false, "Invoice not found.");

            // ── Business Rule 1: Already fully paid ───────────────────────
            decimal alreadyPaid = await _paymentRepo.GetTotalPaidByInvoiceIdAsync(payment.InvoiceId);
            decimal balanceDue = invoice.TotalAmount - alreadyPaid;

            if (balanceDue <= 0)
                return (false, "This invoice is already fully paid.");

            // ── Business Rule 2: Overpayment check ───────────────────────
            if (payment.AmountPaid > balanceDue)
                return (false, $"Payment amount (₹{payment.AmountPaid:N2}) cannot exceed balance due (₹{balanceDue:N2}).");

            // Save the payment record
            await _paymentRepo.AddAsync(payment);

            // ── Business Rule 3: Auto-update invoice status ───────────────
            decimal newTotalPaid = alreadyPaid + payment.AmountPaid;

            if (newTotalPaid >= invoice.TotalAmount)
                invoice.Status = InvoiceStatus.Paid;
            else if (newTotalPaid > 0)
                invoice.Status = InvoiceStatus.Partial;
            else
                invoice.Status = InvoiceStatus.Unpaid;

            await _invoiceRepo.UpdateAsync(invoice);

            return (true, null);
        }
    }

    /// <summary>
    /// DashboardService — assembles the DashboardViewModel for the home page.
    /// Calls DashboardRepository for all aggregate queries.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepo;

        public DashboardService(IDashboardRepository dashboardRepo)
        {
            _dashboardRepo = dashboardRepo;
        }

        /// <summary>
        /// Builds the complete dashboard data in parallel where possible.
        /// Returns a fully populated DashboardViewModel ready for the view.
        /// </summary>
        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            // Run independent queries in parallel for better performance
            var revenueTask = await _dashboardRepo.GetTotalRevenueAsync();
            var pendingTask = await _dashboardRepo.GetTotalPendingAsync();
            var paidCountTask = await _dashboardRepo.GetInvoiceCountByStatusAsync(InvoiceStatus.Paid);
            var unpaidCountTask = await _dashboardRepo.GetInvoiceCountByStatusAsync(InvoiceStatus.Unpaid);
            var partialCountTask = await _dashboardRepo.GetInvoiceCountByStatusAsync(InvoiceStatus.Partial);
            var recentInvoicesTask = await _dashboardRepo.GetRecentInvoicesAsync(5);

            
            return new DashboardViewModel
            {
                TotalRevenue = revenueTask,
                TotalPending = pendingTask,
                TotalPaidInvoices = paidCountTask,
                TotalUnpaidInvoices = unpaidCountTask,
                TotalPartialInvoices = partialCountTask,
                TotalInvoices = paidCountTask + unpaidCountTask + partialCountTask,
                RecentInvoices = recentInvoicesTask.ToList()
            };
        }
    }
}