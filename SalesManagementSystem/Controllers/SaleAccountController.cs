using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleAccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleAccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _context.SaleAccounts
            .Include(x => x.Platform)
            .OrderByDescending(x => x.AccountId)
            .ToListAsync();
        return View(accounts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var account = await _context.SaleAccounts
            .Include(x => x.Platform)
            .FirstOrDefaultAsync(x => x.AccountId == id);
        return account == null ? NotFound() : View(account);
    }

    public async Task<IActionResult> Create()
    {
        await PopulatePlatforms();
        return View(new SaleAccount { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleAccount account)
    {
        if (!ModelState.IsValid)
        {
            await PopulatePlatforms(account.PlatformId);
            return View(account);
        }

        _context.Add(account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var account = await _context.SaleAccounts.FindAsync(id);
        if (account == null) return NotFound();
        await PopulatePlatforms(account.PlatformId);
        return View(account);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaleAccount account)
    {
        if (id != account.AccountId) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulatePlatforms(account.PlatformId);
            return View(account);
        }

        _context.Update(account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var account = await _context.SaleAccounts
            .Include(x => x.Platform)
            .FirstOrDefaultAsync(x => x.AccountId == id);
        return account == null ? NotFound() : View(account);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var account = await _context.SaleAccounts.FindAsync(id);
        if (account == null) return NotFound();

        _context.SaleAccounts.Remove(account);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulatePlatforms(int? selectedId = null)
    {
        var platforms = await _context.SalePlatforms
            .OrderBy(x => x.PlatformName)
            .ToListAsync();
        ViewBag.Platforms = new SelectList(platforms, "PlatformId", "PlatformName", selectedId);
    }
}

