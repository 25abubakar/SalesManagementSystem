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
        => View(await _context.SaleTransactionTypes.OrderByDescending(x => x.TransactionId).ToListAsync());

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
}

