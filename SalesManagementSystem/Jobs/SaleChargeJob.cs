using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Jobs;

public interface ISaleChargeJob
{
    Task CreateAutoChargeIfMissingAsync(long saleId);
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

        if (sale == null)
        {
            return;
        }

        var hasCharge = await _context.SaleCharges.AnyAsync(x => x.SaleId == saleId);
        if (hasCharge)
        {
            return;
        }

        var soldAmount = sale.SoldAmount ?? 0m;
        if (soldAmount <= 0)
        {
            return;
        }

        var defaultChargeType = await _context.SaleChargeTypes
            .AsNoTracking()
            .OrderBy(x => x.ChargeTypeId)
            .FirstOrDefaultAsync();

        if (defaultChargeType == null)
        {
            var createdChargeType = new SaleChargeType
            {
                ChargeTypeName = "Auto Charge",
                IsActive = true
            };

            _context.SaleChargeTypes.Add(createdChargeType);
            await _context.SaveChangesAsync();
            defaultChargeType = createdChargeType;
        }

        _context.SaleCharges.Add(new SaleCharge
        {
            SaleId = saleId,
            ChargeTypeId = defaultChargeType.ChargeTypeId,
            Amount = soldAmount,
            Remarks = "Auto charge from SoldAmount after 10 minutes."
        });

        await _context.SaveChangesAsync();
    }
}
