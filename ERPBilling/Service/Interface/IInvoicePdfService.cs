using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface IInvoicePdfService
    {
        byte[] GenerateInvoicePdf(Invoice invoice);
    }
}
