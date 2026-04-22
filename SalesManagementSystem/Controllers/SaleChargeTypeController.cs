using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleChargeTypeController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleChargeTypeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
        => View(await _context.SaleChargeTypes.OrderByDescending(x => x.ChargeTypeId).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var type = await _context.SaleChargeTypes.FirstOrDefaultAsync(x => x.ChargeTypeId == id);
        return type == null ? NotFound() : View(type);
    }

    public IActionResult Create() => View(new SaleChargeType { IsActive = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleChargeType type)
    {
        type.ChargeTypeName = type.ChargeTypeName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(type.ChargeTypeName))
        {
            ModelState.AddModelError(nameof(type.ChargeTypeName), "Charge type already exists.");
        }

        if (!ModelState.IsValid) return View(type);
        _context.Add(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var type = await _context.SaleChargeTypes.FindAsync(id);
        return type == null ? NotFound() : View(type);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaleChargeType type)
    {
        if (id != type.ChargeTypeId) return BadRequest();
        type.ChargeTypeName = type.ChargeTypeName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(type.ChargeTypeName, type.ChargeTypeId))
        {
            ModelState.AddModelError(nameof(type.ChargeTypeName), "Charge type already exists.");
        }

        if (!ModelState.IsValid) return View(type);

        _context.Update(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var type = await _context.SaleChargeTypes.FirstOrDefaultAsync(x => x.ChargeTypeId == id);
        return type == null ? NotFound() : View(type);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var type = await _context.SaleChargeTypes.FindAsync(id);
        if (type == null) return NotFound();

        _context.SaleChargeTypes.Remove(type);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private Task<bool> IsDuplicateName(string name, int? excludeId = null)
    {
        var normalized = name.ToLower();
        return _context.SaleChargeTypes.AnyAsync(x =>
            (!excludeId.HasValue || x.ChargeTypeId != excludeId.Value) &&
            x.ChargeTypeName.ToLower() == normalized);
    }
}

