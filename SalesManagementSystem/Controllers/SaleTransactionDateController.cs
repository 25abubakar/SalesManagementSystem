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

        public async Task<IActionResult> Index(long? saleId = null)
        {
            var query = _context.SaleTransactionDates
                .Include(x => x.SaleAcct)
                .Include(x => x.SaleDate)
                .AsQueryable();

            if (saleId.HasValue)
            {
                query = query.Where(x => x.SaleAcctId == saleId.Value);
            }

            var data = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            ViewBag.SaleId = saleId;
            return View(data);
        }

        public IActionResult Create(long? saleId = null)
        {
            ViewBag.SaleTransactions = new SelectList(
                _context.SaleAccts, "Id", "OrderID", saleId);

            ViewBag.DateLabels = new SelectList(
                _context.SaleDates, "Id", "DateLabel");

            return View(new SaleTransactionDate
            {
                SaleAcctId = saleId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleTransactionDate model)
        {
            if (ModelState.IsValid)
            {
                model.Id = 0;

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { saleId = model.SaleAcctId });
            }

            ViewBag.SaleTransactions = new SelectList(
                _context.SaleAccts, "Id", "OrderID", model.SaleAcctId);

            ViewBag.DateLabels = new SelectList(
                _context.SaleDates, "Id", "DateLabel", model.DateLabelId);

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _context.SaleTransactionDates.FindAsync(id);
            if (entity == null) return NotFound();

            ViewBag.SaleTransactions = new SelectList(
                _context.SaleAccts, "Id", "OrderID", entity.SaleAcctId);

            ViewBag.DateLabels = new SelectList(
                _context.SaleDates, "Id", "DateLabel", entity.DateLabelId);

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SaleTransactionDate model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SaleTransactions = new SelectList(
                _context.SaleAccts, "Id", "OrderID", model.SaleAcctId);

            ViewBag.DateLabels = new SelectList(
                _context.SaleDates, "Id", "DateLabel", model.DateLabelId);

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.SaleTransactionDates
                .Include(x => x.SaleAcct)
                .Include(x => x.SaleDate)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null) return NotFound();

            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.SaleTransactionDates.FindAsync(id);
            if (entity != null)
            {
                _context.SaleTransactionDates.Remove(entity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}