using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers;

public class SaleChargeController : Controller
{
    private readonly ApplicationDbContext _context;

    public SaleChargeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(long? saleId = null)
    {
        var query = _context.SaleCharges
            .Include(x => x.Sale)
            .ThenInclude(s => s!.Product)
            .Include(x => x.ChargeType)
            .AsQueryable();

        if (saleId.HasValue)
            query = query.Where(x => x.SaleId == saleId.Value);

        var charges = await query
            .OrderByDescending(x => x.SaleChargeId)
            .ToListAsync();

        ViewBag.SaleId = saleId;
        return View(charges);
    }

    public async Task<IActionResult> Details(long id)
    {
        var charge = await _context.SaleCharges
            .Include(x => x.Sale)
            .ThenInclude(s => s!.Product)
            .Include(x => x.ChargeType)
            .FirstOrDefaultAsync(x => x.SaleChargeId == id);

        return charge == null ? NotFound() : View(charge);
    }

    public async Task<IActionResult> Create(long? saleId = null)
    {
        await PopulateDropDowns(saleId);
        return View(new SaleCharge { SaleId = saleId ?? 0, Amount = 0m });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaleCharge charge)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropDowns(charge.SaleId);
            return View(charge);
        }

        _context.Add(charge);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { saleId = charge.SaleId });
    }

    public async Task<IActionResult> Edit(long id)
    {
        var charge = await _context.SaleCharges.FindAsync(id);
        if (charge == null) return NotFound();
        await PopulateDropDowns(charge.SaleId, charge.ChargeTypeId);
        return View(charge);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, SaleCharge charge)
    {
        if (id != charge.SaleChargeId) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateDropDowns(charge.SaleId, charge.ChargeTypeId);
            return View(charge);
        }

        _context.Update(charge);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { saleId = charge.SaleId });
    }

    public async Task<IActionResult> Delete(long id)
    {
        var charge = await _context.SaleCharges
            .Include(x => x.Sale)
            .ThenInclude(s => s!.Product)
            .Include(x => x.ChargeType)
            .FirstOrDefaultAsync(x => x.SaleChargeId == id);

        return charge == null ? NotFound() : View(charge);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var charge = await _context.SaleCharges.FindAsync(id);
        if (charge == null) return NotFound();

        var saleId = charge.SaleId;
        _context.SaleCharges.Remove(charge);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { saleId });
    }

    private async Task PopulateDropDowns(long? saleId = null, int? chargeTypeId = null)
    {
        var sales = await _context.SaleAccts
            .Include(x => x.Product)
            .OrderByDescending(x => x.Id)
            .Select(x => new
            {
                x.Id,
                Display = (x.OrderID ?? ("Sale#" + x.Id)) + " (" + (x.Product != null ? x.Product.ProductName : "") + ")"
            })
            .ToListAsync();

        ViewBag.Sales = new SelectList(sales, "Id", "Display", saleId);

        var chargeTypes = await _context.SaleChargeTypes
            .OrderBy(x => x.ChargeTypeName)
            .ToListAsync();
        ViewBag.ChargeTypes = new SelectList(chargeTypes, "ChargeTypeId", "ChargeTypeName", chargeTypeId);
    }
}

