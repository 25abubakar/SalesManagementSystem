using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using SalesManagementSystem.Filters;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;
using System.Diagnostics;

namespace SalesManagementSystem.Controllers
{
    //[SessionAuthorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var salesQuery = _context.SaleAccts.AsNoTracking();
            var totalSales = await salesQuery.CountAsync();
            var totalSoldAmount = await salesQuery.SumAsync(x => x.SoldAmount ?? 0m);
            var totalProfitAmount = await salesQuery.SumAsync(x => x.ProfitAmount ?? 0m);

            var monthlySales = await salesQuery
                .Where(x => x.CreatedDate.HasValue)
                .GroupBy(x => new {  x.CreatedDate!.Value.Month, x.CreatedDate!.Value.Year,})
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                
                .Select(g => new DashboardChartPoint
                {
                    Label = $"{g.Key.Month:00}-{g.Key.Year}",
                    SoldAmount = g.Sum(x => x.SoldAmount ?? 0m),
                    ProfitAmount = g.Sum(x => x.ProfitAmount ?? 0m)
                })
                .ToListAsync();

            var recentSales = await _context.SaleAccts
                .AsNoTracking()
                .Include(x => x.Platform)
                .Include(x => x.Product)
                .Include(x => x.StatusMaster)
                .OrderByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                .ThenByDescending(x => x.Id)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalSales = totalSales,
                TotalSoldAmount = totalSoldAmount,
                TotalProfitAmount = totalProfitAmount,
                AverageProfitAmount = totalSales == 0 ? 0m : totalProfitAmount / totalSales,
                MonthlySales = monthlySales,
                RecentSales = recentSales
            };

            return View(model);
        }

        public IActionResult NewJob()
        {
            var JobId = BackgroundJob.Schedule(() => SendNotification("Welcome HangFire"), TimeSpan.FromSeconds(15));
            return View();
        }


        public void SendNotification(string v)
        {
            Console.WriteLine(v);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
