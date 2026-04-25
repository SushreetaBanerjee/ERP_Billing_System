using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface IPaymentService
    {
        // Returns the invoice + calculated balance info for the payment form
        Task<(Invoice? Invoice, decimal TotalPaid, decimal BalanceDue)> GetPaymentFormDataAsync(int invoiceId);

        // Records payment and updates invoice status
        // Returns (success, errorMessage)
        Task<(bool Success, string? Error)> RecordPaymentAsync(Payment payment);
    }
}
