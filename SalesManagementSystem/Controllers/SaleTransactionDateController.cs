using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers
{
    public class SaleTransactionDateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleTransactionDateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.SaleTransactionDates
                .Include(x => x.SaleDate)
                .Include(x => x.SaleAcct)
                .ToList();

            return View(data);
        }

       public IActionResult Create()
{
    var vm = new SaleTransactionVM();

    vm.SaleDateList = _context.SaleDates
        .Select(x => new SelectListItem
        {
            Value = x.SaleDateId.ToString(),
            Text = "SaleDate #" + x.SaleDateId
        }).ToList();

    vm.SaleAcctList = _context.SaleAccts
        .Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = "Order: " + (x.OrderID ?? "No OrderID") + " (ID: " + x.Id + ")"
        }).ToList();

    return View(vm);
}

        [HttpPost]
        public async Task<IActionResult> Create(SaleTransactionVM vm)
        {
            var entity = new SaleTransactionDate
            {
                SaleDateId = vm.SaleDateId,
                SaleAcctId = vm.SaleAcctId,
                Date = vm.Date
            };

            _context.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}