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
            .OrderBy(x => x.ProductId)
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
        await AssignCompanyFromPlatformAsync(product);
        product.ProductName = product.ProductName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(product.ProductName, product.PlatformId))
        {
            ModelState.AddModelError(nameof(product.ProductName), "Product already exists for this platform.");
        }

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
        await AssignCompanyFromPlatformAsync(product);
        product.ProductName = product.ProductName?.Trim() ?? string.Empty;
        if (await IsDuplicateName(product.ProductName, product.PlatformId, product.ProductId))
        {
            ModelState.AddModelError(nameof(product.ProductName), "Product already exists for this platform.");
        }

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
        ViewBag.PlatformOptions = platforms;
    }

    private async Task AssignCompanyFromPlatformAsync(SaleProduct product)
    {
        if (!product.PlatformId.HasValue)
        {
            return;
        }

        var platform = await _context.SalePlatforms
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PlatformId == product.PlatformId.Value);

        if (platform == null)
        {
            ModelState.AddModelError(nameof(product.PlatformId), "Selected platform does not exist.");
            return;
        }

        if (!platform.CompanyId.HasValue)
        {
            ModelState.AddModelError(nameof(product.CompanyId), "Selected platform has no Company ID.");
            return;
        }

        product.CompanyId = platform.CompanyId.Value;
    }

    private Task<bool> IsDuplicateName(string name, int? platformId, int? excludeId = null)
    {
        var normalized = name.ToLower();
        return _context.SaleProducts.AnyAsync(x =>
            (!excludeId.HasValue || x.ProductId != excludeId.Value) &&
            x.PlatformId == platformId &&
            x.ProductName.ToLower() == normalized);
    }
}

