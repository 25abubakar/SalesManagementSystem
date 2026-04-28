using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Jobs;

public interface ISaleChargeJob
{
    Task CreateAutoChargeIfMissingAsync(long saleId);
    Task CreateAutoTransactionDateIfMissingAsync(long saleId);
    Task CreateAutoChargeAndDateIfMissingAsync(long saleId);
}

public class SaleChargeJob : ISaleChargeJob
{
    private readonly ApplicationDbContext _context;

    public SaleChargeJob(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateAutoChargeIfMissingAsync(long saleId)
    {
        var sale = await _context.SaleAccts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == saleId);

        if (sale == null) return;

        var hasCharge = await _context.SaleCharges.AnyAsync(x => x.SaleId == saleId);
        if (hasCharge) return;

        var defaultChargeType = await GetOrCreateDefaultChargeTypeAsync();
        if (defaultChargeType == null) return;

        var amount = sale.SoldAmount ?? 0m;

        _context.SaleCharges.Add(new SaleCharge
        {
            SaleId = saleId,
            ChargeTypeId = defaultChargeType.ChargeTypeId,
            Amount = amount,
            Remarks = $"Auto Generate Charge created on {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Auto-creates a TransactionDate row (using today's date) if the sale has no transaction dates.
    /// Uses DateLabelId = 1 (TransactionDate) as the default label.
    /// </summary>
    public async Task CreateAutoTransactionDateIfMissingAsync(long saleId)
    {
        var sale = await _context.SaleAccts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == saleId);

        if (sale == null) return;

        var hasDate = await _context.SaleTransactionDates.AnyAsync(x => x.SaleAcctId == saleId);
        if (hasDate) return;

        // Use the first available SaleDate label (TransactionDate = Id 1 by seed data)
        var defaultDateLabel = await _context.SaleDates
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync();

        if (defaultDateLabel == null) return;

        _context.SaleTransactionDates.Add(new SaleTransactionDate
        {
            SaleAcctId = saleId,
            DateLabelId = defaultDateLabel.Id,
            Date = DateTime.Today
        });

        await _context.SaveChangesAsync();
    }

    public async Task CreateAutoChargeAndDateIfMissingAsync(long saleId)
    {
        await CreateAutoChargeIfMissingAsync(saleId);
        await CreateAutoTransactionDateIfMissingAsync(saleId);
    }

    private async Task<SaleChargeType?> GetOrCreateDefaultChargeTypeAsync()
    {
        var chargeType = await _context.SaleChargeTypes
            .AsNoTracking()
            .OrderBy(x => x.ChargeTypeId)
            .FirstOrDefaultAsync();

        if (chargeType != null) return chargeType;

        // No charge types exist at all — create a default one
        var newChargeType = new SaleChargeType
        {
            ChargeTypeName = "Auto Charge",
            IsActive = true
        };

        _context.SaleChargeTypes.Add(newChargeType);
        await _context.SaveChangesAsync();

        return newChargeType;
    }
}
