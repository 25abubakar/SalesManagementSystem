using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Models;
using SalesManagementSystem.Data;

namespace SalesManagementSystem.Controllers
{
    public class SaleAcctController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleAcctController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .Include(s => s.TransactionType)
                .Include(s => s.FromAccount)
                .Include(s => s.ToAccount)
                .Include(s => s.StatusMaster)
                .OrderByDescending(s => s.CreatedDate)
                .ToListAsync();

            return View(sales);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleAcct sale)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    sale.CreatedDate ??= DateTime.Now;

                    _context.Add(sale);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Sale record saved successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Error: " + ex.Message);
                }
            }

            await PopulateDropDowns(sale);
            return View(sale);
        }

        private async Task PopulateDropDowns(SaleAcct? sale = null)
        {
            ViewBag.Platforms = new SelectList(await _context.SalePlatforms.ToListAsync(), "PlatformId", "PlatformName", sale?.PlatformId);
            ViewBag.Products = new SelectList(await _context.SaleProducts.ToListAsync(), "ProductId", "ProductName", sale?.ProductId);
            ViewBag.TransTypes = new SelectList(await _context.SaleTransactionTypes.ToListAsync(), "TransactionId", "TransactionName", sale?.TransactionId);

            var accounts = await _context.SaleAccounts.ToListAsync();
            ViewBag.FromAccounts = new SelectList(accounts, "AccountId", "AccountName", sale?.FromAccountId);
            ViewBag.ToAccounts = new SelectList(accounts, "AccountId", "AccountName", sale?.ToAccountId);

            ViewBag.Statuses = new SelectList(await _context.SaleStatuses.ToListAsync(), "StatusID", "StatusName", sale?.StatusID);
        }

        public async Task<IActionResult> Edit(long id)
        {
            var sale = await _context.SaleAccts.FindAsync(id);
            if (sale == null) return NotFound();

            await PopulateDropDowns(sale);
            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SaleAcct sale)
        {
            if (id != sale.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sale record updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.SaleAccts.AnyAsync(x => x.Id == id)) return NotFound();
                    throw;
                }
            }

            await PopulateDropDowns(sale);
            return View(sale);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var sale = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sale = await _context.SaleAccts.FindAsync(id);
            if (sale == null) return NotFound();

            _context.SaleAccts.Remove(sale);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Sale record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}