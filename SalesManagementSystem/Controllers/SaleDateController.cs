using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers
{
    public class SaleDateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleDateController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🔹 LIST
        public async Task<IActionResult> Index()
        {
            var list = await _context.SaleDates.ToListAsync();
            return View(list);
        }

        public IActionResult Create()
        {
            TempData["Error"] = "Date labels are system defined and cannot be created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Create(SaleDate saleDate)
        {
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _context.SaleDates.FindAsync(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleDate saleDate)
        {
            if (id != saleDate.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(saleDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(saleDate);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.SaleDates.FindAsync(id);

            if (data == null)
                return NotFound();

            bool isUsed = await _context.SaleTransactionDates
                .AnyAsync(x => x.DateLabelId == id);

            if (isUsed)
            {
                TempData["Error"] = "This Date Label is used in transactions and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var data = await _context.SaleDates.FindAsync(id);

            if (data != null)
            {
                _context.SaleDates.Remove(data);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}