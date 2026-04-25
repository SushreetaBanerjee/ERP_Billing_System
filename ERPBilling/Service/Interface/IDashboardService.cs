using ERPBilling.Models;

namespace ERPBilling.Service.Interface
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
