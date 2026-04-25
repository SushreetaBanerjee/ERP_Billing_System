using ERPBilling.Data;
using ERPBilling.Models;
using ERPBilling.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERPBilling.Controllers
{
    /// <summary>
    /// HomeController — serves the Dashboard page only.
    /// Calculates summary stats from the database and passes
    /// them to the Dashboard view via DashboardViewModel.
    /// </summary>
    public class HomeController : Controller
    {
        // DbContext injected via constructor — used to query the database
        private readonly IDashboardService _dashboardService;
        public HomeController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        public async Task<IActionResult> Index()
        {
            var vm = await _dashboardService.GetDashboardDataAsync();
            return View(vm);
        }
    }
}