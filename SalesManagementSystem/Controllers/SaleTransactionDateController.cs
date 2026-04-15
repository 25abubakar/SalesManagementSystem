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
        public IActionResult Edit(int id)
        {
            var entity = _context.SaleTransactionDates.Find(id);
            if (entity == null) return NotFound();

            var vm = new SaleTransactionVM
            {
                SaleDateId = entity.SaleDateId,
                SaleAcctId = entity.SaleAcctId,

                SaleDateList = _context.SaleDates.Select(x => new SelectListItem
                {
                    Value = x.SaleDateId.ToString(),
                    Text = "SaleDate #" + x.SaleDateId
                }).ToList(),

                SaleAcctList = _context.SaleAccts.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = "Order: " + (x.OrderID ?? "No OrderID") + " (ID: " + x.Id + ")"
                }).ToList()
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SaleTransactionVM vm)
        {
            var entity = await _context.SaleTransactionDates.FindAsync(vm.SaleDateId);
            if (entity == null) return NotFound();

            entity.SaleDateId = vm.SaleDateId;
            entity.SaleAcctId = vm.SaleAcctId;
            entity.Date = vm.Date;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var data = _context.SaleTransactionDates
                .Include(x => x.SaleDate)
                .Include(x => x.SaleAcct)
                .FirstOrDefault(x => x.SaleTransId == id);

            if (data == null) return NotFound();

            return View(data);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.SaleTransactionDates.FindAsync(id);
            if (entity == null) return NotFound();

            _context.SaleTransactionDates.Remove(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}