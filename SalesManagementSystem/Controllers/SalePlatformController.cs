using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SalePlatformController : Controller
{
    private readonly ApplicationDbContext _context;

    public SalePlatformController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
        => View(await _context.SalePlatforms.OrderByDescending(x => x.PlatformId).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var platform = await _context.SalePlatforms.FirstOrDefaultAsync(x => x.PlatformId == id);
        return platform == null ? NotFound() : View(platform);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SalePlatform platform)
    {
        if (!ModelState.IsValid) return View(platform);
        _context.Add(platform);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var platform = await _context.SalePlatforms.FindAsync(id);
        return platform == null ? NotFound() : View(platform);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SalePlatform platform)
    {
        if (id != platform.PlatformId) return BadRequest();
        if (!ModelState.IsValid) return View(platform);

        _context.Update(platform);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var platform = await _context.SalePlatforms.FirstOrDefaultAsync(x => x.PlatformId == id);
        return platform == null ? NotFound() : View(platform);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var platform = await _context.SalePlatforms.FindAsync(id);
        if (platform == null) return NotFound();

        _context.SalePlatforms.Remove(platform);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

