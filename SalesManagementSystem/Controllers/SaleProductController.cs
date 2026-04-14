using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.SaleProducts
            .Include(x => x.Platform)
            .OrderByDescending(x => x.ProductId)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.SaleProducts
            .Include(x => x.Platform)
            .FirstOrDefaultAsync(x => x.ProductId == id);

        return product == null ? NotFound() : View(product);
    }

    public async Task<IActionResult> Create()
    {
        await PopulatePlatforms();
        return View(new SaleProduct { IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleProduct product)
    {
        if (!ModelState.IsValid)
        {
            await PopulatePlatforms(product.PlatformId);
            return View(product);
        }

        _context.Add(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.SaleProducts.FindAsync(id);
        if (product == null) return NotFound();
        await PopulatePlatforms(product.PlatformId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SaleProduct product)
    {
        if (id != product.ProductId) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulatePlatforms(product.PlatformId);
            return View(product);
        }

        _context.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.SaleProducts
            .Include(x => x.Platform)
            .FirstOrDefaultAsync(x => x.ProductId == id);

        return product == null ? NotFound() : View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.SaleProducts.FindAsync(id);
        if (product == null) return NotFound();

        _context.SaleProducts.Remove(product);
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

