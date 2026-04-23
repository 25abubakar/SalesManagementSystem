using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleTransactionTypeController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleTransactionTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
        => View(await _context.SaleTransactionTypes.OrderBy(x => x.TransactionId).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var type = await _context.SaleTransactionTypes.FirstOrDefaultAsync(x => x.TransactionId == id);
        return type == null ? NotFound() : View(type);
    }

    public IActionResult Create() => View(new SaleTransactionType { IsActive = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleTransactionType type)
    {
        type.TransactionName = type.TransactionName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(type.TransactionName))
        {
            ModelState.AddModelError(nameof(type.TransactionName), "Transaction type already exists.");
        }

        if (!ModelState.IsValid) return View(type);
        _context.Add(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var type = await _context.SaleTransactionTypes.FindAsync(id);
        return type == null ? NotFound() : View(type);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaleTransactionType type)
    {
        if (id != type.TransactionId) return BadRequest();
        type.TransactionName = type.TransactionName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(type.TransactionName, type.TransactionId))
        {
            ModelState.AddModelError(nameof(type.TransactionName), "Transaction type already exists.");
        }

        if (!ModelState.IsValid) return View(type);

        _context.Update(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var type = await _context.SaleTransactionTypes.FirstOrDefaultAsync(x => x.TransactionId == id);
        return type == null ? NotFound() : View(type);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var type = await _context.SaleTransactionTypes.FindAsync(id);
        if (type == null) return NotFound();

        _context.SaleTransactionTypes.Remove(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private Task<bool> IsDuplicateName(string name, int? excludeId = null)
    {
        var normalized = name.ToLower();
        return _context.SaleTransactionTypes.AnyAsync(x =>
            (!excludeId.HasValue || x.TransactionId != excludeId.Value) &&
            x.TransactionName.ToLower() == normalized);
    }
}

