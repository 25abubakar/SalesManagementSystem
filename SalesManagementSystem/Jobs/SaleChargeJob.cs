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

    /// <summary>
    /// Auto-creates a charge row if the sale has no charges.
    /// ChargeType name will match the sale's TransactionType name.
    /// If no matching ChargeType exists, it gets created automatically.
    /// Falls back to first available ChargeType if sale has no TransactionType.
    /// </summary>
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

    /// <summary>
    /// Auto-creates a TransactionDate row (today's date) if the sale has no transaction dates.
    /// Uses the first SaleDate label (TransactionDate) from seed data.
    /// </summary>
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

    /// <summary>
    /// Combined job: auto-creates both charge and transaction date if either is missing.
    /// This is the main entry point scheduled from the controller.
    /// </summary>
    public async Task CreateAutoChargeAndDateIfMissingAsync(long saleId)
    {
        await CreateAutoChargeIfMissingAsync(saleId);
        await CreateAutoTransactionDateIfMissingAsync(saleId);
    }

    // -----------------------------------------------------------------------
    // Private helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Finds a ChargeType whose name matches the sale's TransactionType name.
    /// If not found, creates a new ChargeType with that name.
    /// If transactionName is null/empty, falls back to the first existing ChargeType.
    /// </summary>
    private async Task<SaleChargeType?> GetOrCreateChargeTypeMatchingTransactionAsync(string? transactionName)
    {
        // If the sale has a TransactionType, try to match or create a ChargeType with the same name
        if (!string.IsNullOrWhiteSpace(transactionName))
        {
            var matched = await _context.SaleChargeTypes
                .FirstOrDefaultAsync(x => x.ChargeTypeName == transactionName);

            if (matched != null) return matched;

            // No matching ChargeType — create one with the same name as the TransactionType
            var newChargeType = new SaleChargeType
            {
                ChargeTypeName = transactionName,
                IsActive = true
            };

            _context.SaleChargeTypes.Add(newChargeType);
            await _context.SaveChangesAsync();

            return newChargeType;
        }

        // No TransactionType on the sale — fall back to first existing ChargeType
        var fallback = await _context.SaleChargeTypes
            .OrderBy(x => x.ChargeTypeId)
            .FirstOrDefaultAsync();

        if (fallback != null) return fallback;

        // No ChargeTypes exist at all — create a generic default
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
