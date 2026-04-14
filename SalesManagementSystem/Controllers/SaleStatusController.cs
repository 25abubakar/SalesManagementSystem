using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleStatusController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleStatusController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
        => View(await _context.SaleStatuses.OrderByDescending(x => x.StatusID).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var status = await _context.SaleStatuses.FirstOrDefaultAsync(x => x.StatusID == id);
        return status == null ? NotFound() : View(status);
    }

    public IActionResult Create() => View(new SaleStatus { IsActive = true });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleStatus status)
    {
        if (!ModelState.IsValid) return View(status);
        _context.Add(status);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var status = await _context.SaleStatuses.FindAsync(id);
        return status == null ? NotFound() : View(status);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaleStatus status)
    {
        if (id != status.StatusID) return BadRequest();
        if (!ModelState.IsValid) return View(status);

        _context.Update(status);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var status = await _context.SaleStatuses.FirstOrDefaultAsync(x => x.StatusID == id);
        return status == null ? NotFound() : View(status);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var status = await _context.SaleStatuses.FindAsync(id);
        if (status == null) return NotFound();

        _context.SaleStatuses.Remove(status);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

