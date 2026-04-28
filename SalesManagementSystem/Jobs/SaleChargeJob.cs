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
            .Include(x => x.TransactionType)
            .FirstOrDefaultAsync(x => x.Id == saleId);

        if (sale == null) return;

        var hasCharge = await _context.SaleCharges.AnyAsync(x => x.SaleId == saleId);
        if (hasCharge) return;

        var chargeType = await GetOrCreateChargeTypeMatchingTransactionAsync(sale.TransactionType?.TransactionName);
        if (chargeType == null) return;

        var amount = sale.SoldAmount ?? 0m;

        _context.SaleCharges.Add(new SaleCharge
        {
            SaleId = saleId,
            ChargeTypeId = chargeType.ChargeTypeId,
            Amount = amount,
            Remarks = $"Auto charge created on {DateTime.Now:yyyy-MM-dd HH:mm:ss} (no charge was added at creation)."
        });

        await _context.SaveChangesAsync();
    }

    public async Task CreateAutoTransactionDateIfMissingAsync(long saleId)
    {
        var sale = await _context.SaleAccts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == saleId);

        if (sale == null) return;

        var hasDate = await _context.SaleTransactionDates.AnyAsync(x => x.SaleAcctId == saleId);
        if (hasDate) return;

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

    private async Task<SaleChargeType?> GetOrCreateChargeTypeMatchingTransactionAsync(string? transactionName)
    {
        if (!string.IsNullOrWhiteSpace(transactionName))
        {
            var matched = await _context.SaleChargeTypes
                .FirstOrDefaultAsync(x => x.ChargeTypeName == transactionName);

            if (matched != null) return matched;

            var newChargeType = new SaleChargeType
            {
                ChargeTypeName = transactionName,
                IsActive = true
            };

            _context.SaleChargeTypes.Add(newChargeType);
            await _context.SaveChangesAsync();

            return newChargeType;
        }

        var fallback = await _context.SaleChargeTypes
            .OrderBy(x => x.ChargeTypeId)
            .FirstOrDefaultAsync();

        if (fallback != null) return fallback;

        var defaultChargeType = new SaleChargeType
        {
            ChargeTypeName = "Auto Charge",
            IsActive = true
        };

        _context.SaleChargeTypes.Add(defaultChargeType);
        await _context.SaveChangesAsync();

        return defaultChargeType;
    }
}
