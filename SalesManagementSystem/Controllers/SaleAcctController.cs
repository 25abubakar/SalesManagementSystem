using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Hubs;
using SalesManagementSystem.Models;
using SalesManagementSystem.Data;

namespace SalesManagementSystem.Controllers
{
    public class SaleAcctController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SalesHub> _salesHub;

        public SaleAcctController(ApplicationDbContext context, IHubContext<SalesHub> salesHub)
        {
            _context = context;
            _salesHub = salesHub;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .Include(s => s.TransactionType)
                .Include(s => s.FromAccount)
                .Include(s => s.ToAccount)
                .Include(s => s.StatusMaster)
                .Include(s => s.Charges)
                .ThenInclude(c => c.ChargeType)
                .Include(s => s.SaleTransactionDates!)
                .ThenInclude(d => d.SaleDate)
                .OrderBy(s => s.CreatedDate)
                .ToListAsync();

            await PopulateSqlSummaryColumns(sales);
            return View(sales);
        }

        private async Task PopulateSqlSummaryColumns(List<SaleAcct> sales)
        {
            if (sales.Count == 0)
            {
                return;
            }

            var saleIds = sales.Select(x => x.Id).Distinct().ToList();
            var dateSummaryBySaleId = await _context.SaleTransactionDatePivots
                .Where(x => saleIds.Contains(x.SaleAcctId))
                .ToDictionaryAsync(x => x.SaleAcctId, x => x);

            foreach (var sale in sales)
            {
                sale.ChargeTypesCsv = string.Join(", ", sale.Charges
                    .Select(c => c.ChargeType?.ChargeTypeName ?? c.ChargeTypeId.ToString())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct());

                if (dateSummaryBySaleId.TryGetValue(sale.Id, out var summary))
                {
                    sale.TransactionDatesCsv = BuildTransactionDateSummary(summary);
                }
            }
        }

        private static string BuildTransactionDateSummary(SaleTransactionDatePivot pivot)
        {
            var dateParts = new List<string>();

            if (pivot.TransactionDate.HasValue) dateParts.Add($"TransactionDate: {pivot.TransactionDate.Value:yyyy-MM-dd}");
            if (pivot.PaymentDate.HasValue) dateParts.Add($"PaymentDate: {pivot.PaymentDate.Value:yyyy-MM-dd}");
            if (pivot.OrderDate.HasValue) dateParts.Add($"OrderDate: {pivot.OrderDate.Value:yyyy-MM-dd}");
            if (pivot.ProcessDate.HasValue) dateParts.Add($"ProcessDate: {pivot.ProcessDate.Value:yyyy-MM-dd}");
            if (pivot.SoldDate.HasValue) dateParts.Add($"SoldDate: {pivot.SoldDate.Value:yyyy-MM-dd}");

            return string.Join(", ", dateParts);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDropDowns();
            return View(new SaleAcctCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleAcctCreateVM model)
        {
            var chargeRows = (model.Charges ?? new List<SaleChargeEntryVM>())
                .Where(x => x.ChargeTypeId.HasValue || x.Amount.HasValue || !string.IsNullOrWhiteSpace(x.Remarks))
                .ToList();
            var transactionDates = (model.SaleTransactionDates ?? new List<SaleTransactionDateEntryVM>())
                .Where(x => x.DateLabelId.HasValue || x.Date.HasValue)
                .ToList();

            foreach (var item in chargeRows)
            {
                if (!item.ChargeTypeId.HasValue)
                {
                    ModelState.AddModelError("", "Please select charge type for each charge row.");
                }

                if (!item.Amount.HasValue)
                {
                    ModelState.AddModelError("", "Please enter amount for each charge row.");
                }
            }

            var duplicateChargeTypes = chargeRows
                .Where(x => x.ChargeTypeId.HasValue)
                .GroupBy(x => x.ChargeTypeId!.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateChargeTypes.Count > 0)
            {
                ModelState.AddModelError("", "Each charge type can only be selected once.");
            }

            foreach (var item in transactionDates)
            {
                if (!item.DateLabelId.HasValue)
                {
                    ModelState.AddModelError("", "Please select a date label for each sale transaction date row.");
                }

                if (!item.Date.HasValue)
                {
                    ModelState.AddModelError("", "Please enter a date for each sale transaction date row.");
                }
            }

            var duplicateDateLabels = transactionDates
                .Where(x => x.DateLabelId.HasValue)
                .GroupBy(x => x.DateLabelId!.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateDateLabels.Count > 0)
            {
                ModelState.AddModelError("", "Each transaction date label can only be selected once.");
            }

            await ValidateDuplicateSaleAsync(model);
            await ValidateToAccountPlatformMappingAsync(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var sale = new SaleAcct
                    {
                        CompanyId = model.CompanyId,
                        PlatformId = model.PlatformId,
                        ProductId = model.ProductId,
                        OrderID = model.OrderID,
                        QtyHeld = model.QtyHeld,
                        QtySold = model.QtySold,
                        TransactionId = model.TransactionId,
                        FromAccountId = model.FromAccountId,
                        ToAccountId = model.ToAccountId,
                        TotalProCharges = model.TotalProCharges,
                        AmazonFee = model.AmazonFee,
                        OtherCharges = model.OtherCharges,
                        TotalPromotion = model.TotalPromotion,
                        SoldAmount = model.SoldAmount,
                        TotalRroRebate = model.TotalRroRebate,
                        CostAmount = model.CostAmount,
                        AmzProRef = model.AmzProRef,
                        Status = model.Status,
                        Discription = model.Discription,
                        StatusID = model.StatusID,
                        Action = model.Action,
                        CreatedDate = model.CreatedDate ?? DateTime.Now,
                        Charges = chargeRows
                            .Where(x => x.ChargeTypeId.HasValue && x.Amount.HasValue)
                            .Select(x => new SaleCharge
                            {
                                ChargeTypeId = x.ChargeTypeId!.Value,
                                Amount = x.Amount!.Value,
                                Remarks = x.Remarks
                            })
                            .ToList(),
                        SaleTransactionDates = transactionDates
                            .Where(x => x.DateLabelId.HasValue && x.Date.HasValue)
                            .Select(x => new SaleTransactionDate
                            {
                                DateLabelId = x.DateLabelId!.Value,
                                Date = x.Date!.Value
                            })
                            .ToList()
                    };

                    _context.Add(sale);
                    await _context.SaveChangesAsync();
                    await _salesHub.Clients.All.SendAsync("SalesUpdated", "created", sale.Id);

                    TempData["Success"] = "Sale record saved successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Error: " + ex.Message);
                }
            }

            model.Charges = chargeRows;
            model.SaleTransactionDates = transactionDates;
            await PopulateDropDowns();
            return View(model);
        }

        private async Task PopulateDropDowns(SaleAcct? sale = null)
        {
            ViewBag.Platforms = new SelectList(await _context.SalePlatforms.ToListAsync(), "PlatformId", "PlatformName", sale?.PlatformId);
            ViewBag.Products = new SelectList(await _context.SaleProducts.ToListAsync(), "ProductId", "ProductName", sale?.ProductId);
            ViewBag.TransTypes = new SelectList(await _context.SaleTransactionTypes.ToListAsync(), "TransactionId", "TransactionName", sale?.TransactionId);

            var accounts = await _context.SaleAccounts.ToListAsync();
            ViewBag.FromAccounts = new SelectList(accounts, "AccountId", "AccountName", sale?.FromAccountId);
            ViewBag.ToAccounts = new SelectList(accounts, "AccountId", "AccountName", sale?.ToAccountId);
            ViewBag.ToAccountOptions = accounts;

            ViewBag.Statuses = new SelectList(await _context.SaleStatuses.ToListAsync(), "StatusID", "StatusName", sale?.StatusID);
            ViewBag.DateLabels = new SelectList(await _context.SaleDates.ToListAsync(), "Id", "DateLabel");
            ViewBag.ChargeTypes = new SelectList(await _context.SaleChargeTypes.ToListAsync(), "ChargeTypeId", "ChargeTypeName");
        }

        public async Task<IActionResult> Edit(long id)
        {
            var sale = await _context.SaleAccts
                .Include(s => s.Charges)
                .ThenInclude(c => c.ChargeType)
                .Include(s => s.SaleTransactionDates!)
                .ThenInclude(d => d.SaleDate)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sale == null) return NotFound();

            await PopulateDropDowns(sale);
            var model = new SaleAcctCreateVM
            {
                Id = sale.Id,
                CompanyId = sale.CompanyId,
                PlatformId = sale.PlatformId,
                ProductId = sale.ProductId,
                OrderID = sale.OrderID,
                QtyHeld = sale.QtyHeld,
                QtySold = sale.QtySold,
                TransactionId = sale.TransactionId,
                FromAccountId = sale.FromAccountId,
                ToAccountId = sale.ToAccountId,
                TotalProCharges = sale.TotalProCharges,
                AmazonFee = sale.AmazonFee,
                OtherCharges = sale.OtherCharges,
                TotalPromotion = sale.TotalPromotion,
                SoldAmount = sale.SoldAmount,
                TotalRroRebate = sale.TotalRroRebate,
                CostAmount = sale.CostAmount,
                ProfitAmount = sale.ProfitAmount,
                AmzProRef = sale.AmzProRef,
                Status = sale.Status,
                Discription = sale.Discription,
                StatusID = sale.StatusID,
                Action = sale.Action,
                CreatedDate = sale.CreatedDate,
                Charges = sale.Charges.Select(c => new SaleChargeEntryVM
                {
                    SaleChargeId = c.SaleChargeId,
                    ChargeTypeId = c.ChargeTypeId,
                    Amount = c.Amount,
                    Remarks = c.Remarks
                }).ToList(),
                SaleTransactionDates = sale.SaleTransactionDates?
                    .Select(d => new SaleTransactionDateEntryVM
                    {
                        DateLabelId = d.DateLabelId,
                        Date = d.Date
                    }).ToList() ?? new List<SaleTransactionDateEntryVM>()
            };
            return View(model);
        }

        public async Task<IActionResult> Details(long id)
        {
            var sale = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .Include(s => s.TransactionType)
                .Include(s => s.FromAccount)
                .Include(s => s.ToAccount)
                .Include(s => s.StatusMaster)
                .Include(s => s.Charges)
                .ThenInclude(c => c.ChargeType)
                .Include(s => s.SaleTransactionDates!)
                .ThenInclude(d => d.SaleDate)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }

        public async Task<IActionResult> Print(long id)
        {
            var sale = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .Include(s => s.TransactionType)
                .Include(s => s.FromAccount)
                .Include(s => s.ToAccount)
                .Include(s => s.StatusMaster)
                .Include(s => s.Charges)
                .ThenInclude(c => c.ChargeType)
                .Include(s => s.SaleTransactionDates!)
                .ThenInclude(d => d.SaleDate)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, SaleAcctCreateVM model)
        {
            if (id != model.Id) return BadRequest();

            var chargeRows = (model.Charges ?? new List<SaleChargeEntryVM>())
                .Where(x => x.ChargeTypeId.HasValue || x.Amount.HasValue || !string.IsNullOrWhiteSpace(x.Remarks))
                .ToList();
            var transactionDateRows = (model.SaleTransactionDates ?? new List<SaleTransactionDateEntryVM>())
                .Where(x => x.DateLabelId.HasValue || x.Date.HasValue)
                .ToList();

            foreach (var row in chargeRows)
            {
                if (!row.ChargeTypeId.HasValue)
                {
                    ModelState.AddModelError("", "Please select charge type for each charge row.");
                }

                if (!row.Amount.HasValue)
                {
                    ModelState.AddModelError("", "Please enter amount for each charge row.");
                }
            }

            var duplicateChargeTypes = chargeRows
                .Where(x => x.ChargeTypeId.HasValue)
                .GroupBy(x => x.ChargeTypeId!.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateChargeTypes.Count > 0)
            {
                ModelState.AddModelError("", "Each charge type can only be selected once.");
            }

            foreach (var row in transactionDateRows)
            {
                if (!row.DateLabelId.HasValue)
                {
                    ModelState.AddModelError("", "Please select a date label for each transaction row.");
                }

                if (!row.Date.HasValue)
                {
                    ModelState.AddModelError("", "Please enter date for each transaction row.");
                }
            }

            var duplicateDateLabels = transactionDateRows
                .Where(x => x.DateLabelId.HasValue)
                .GroupBy(x => x.DateLabelId!.Value)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateDateLabels.Count > 0)
            {
                ModelState.AddModelError("", "Each transaction date label can only be selected once.");
            }

            await ValidateDuplicateSaleAsync(model, id);
            await ValidateToAccountPlatformMappingAsync(model);

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSale = await _context.SaleAccts
                        .Include(x => x.Charges)
                        .Include(x => x.SaleTransactionDates)
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (existingSale == null) return NotFound();

                    existingSale.CompanyId = model.CompanyId;
                    existingSale.PlatformId = model.PlatformId;
                    existingSale.ProductId = model.ProductId;
                    existingSale.OrderID = model.OrderID;
                    existingSale.QtyHeld = model.QtyHeld;
                    existingSale.QtySold = model.QtySold;
                    existingSale.TransactionId = model.TransactionId;
                    existingSale.FromAccountId = model.FromAccountId;
                    existingSale.ToAccountId = model.ToAccountId;
                    existingSale.TotalProCharges = model.TotalProCharges;
                    existingSale.AmazonFee = model.AmazonFee;
                    existingSale.OtherCharges = model.OtherCharges;
                    existingSale.TotalPromotion = model.TotalPromotion;
                    existingSale.SoldAmount = model.SoldAmount;
                    existingSale.TotalRroRebate = model.TotalRroRebate;
                    existingSale.CostAmount = model.CostAmount;
                    existingSale.AmzProRef = model.AmzProRef;
                    existingSale.Status = model.Status;
                    existingSale.Discription = model.Discription;
                    existingSale.StatusID = model.StatusID;
                    existingSale.Action = model.Action;
                    existingSale.CreatedDate = model.CreatedDate;

                    var currentCharges = existingSale.Charges.ToDictionary(c => c.SaleChargeId, c => c);
                    var incomingChargeIds = chargeRows.Where(c => c.SaleChargeId.HasValue).Select(c => c.SaleChargeId!.Value).ToHashSet();
                    var chargeIdsToDelete = currentCharges.Keys.Where(existingId => !incomingChargeIds.Contains(existingId)).ToList();
                    foreach (var chargeId in chargeIdsToDelete)
                    {
                        _context.SaleCharges.Remove(currentCharges[chargeId]);
                    }

                    foreach (var row in chargeRows)
                    {
                        if (row.SaleChargeId.HasValue && currentCharges.TryGetValue(row.SaleChargeId.Value, out var existingCharge))
                        {
                            existingCharge.ChargeTypeId = row.ChargeTypeId!.Value;
                            existingCharge.Amount = row.Amount!.Value;
                            existingCharge.Remarks = row.Remarks;
                        }
                        else
                        {
                            existingSale.Charges.Add(new SaleCharge
                            {
                                ChargeTypeId = row.ChargeTypeId!.Value,
                                Amount = row.Amount!.Value,
                                Remarks = row.Remarks
                            });
                        }
                    }

                    var currentDates = existingSale.SaleTransactionDates?.ToList() ?? new List<SaleTransactionDate>();
                    _context.SaleTransactionDates.RemoveRange(currentDates);
                    existingSale.SaleTransactionDates = transactionDateRows
                        .Where(x => x.DateLabelId.HasValue && x.Date.HasValue)
                        .Select(x => new SaleTransactionDate
                        {
                            SaleAcctId = existingSale.Id,
                            DateLabelId = x.DateLabelId!.Value,
                            Date = x.Date!.Value
                        }).ToList();

                    await _context.SaveChangesAsync();
                    await _salesHub.Clients.All.SendAsync("SalesUpdated", "updated", existingSale.Id);
                    TempData["Success"] = "Sale record updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.SaleAccts.AnyAsync(x => x.Id == id)) return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to update record. Error: " + ex.Message);
                }
            }

            model.Charges = chargeRows;
            model.SaleTransactionDates = transactionDateRows;
            await PopulateDropDowns();
            return View(model);
        }

        private async Task ValidateDuplicateSaleAsync(SaleAcctCreateVM model, long? currentSaleId = null)
        {
            if (string.IsNullOrWhiteSpace(model.OrderID))
            {
                return;
            }

            var normalizedOrderId = model.OrderID.Trim().ToLower();
            model.OrderID = model.OrderID.Trim();
            var duplicateExists = await _context.SaleAccts.AnyAsync(x =>
                x.OrderID != null &&
                x.OrderID.Trim().ToLower() == normalizedOrderId &&
                (!currentSaleId.HasValue || x.Id != currentSaleId.Value));

            if (duplicateExists)
            {
                ModelState.AddModelError(nameof(model.OrderID), "This Order ID already exists. Please enter a unique Order ID.");
            }
        }

        private async Task ValidateToAccountPlatformMappingAsync(SaleAcctCreateVM model)
        {
            if (!model.PlatformId.HasValue || !model.ToAccountId.HasValue)
            {
                return;
            }

            var toAccount = await _context.SaleAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AccountId == model.ToAccountId.Value);

            if (toAccount == null)
            {
                ModelState.AddModelError(nameof(model.ToAccountId), "Selected To Account does not exist.");
                return;
            }

            if (toAccount.PlatformId.HasValue && toAccount.PlatformId.Value != model.PlatformId.Value)
            {
                ModelState.AddModelError(nameof(model.ToAccountId), "Selected To Account does not belong to the selected platform.");
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            var sale = await _context.SaleAccts
                .Include(s => s.Platform)
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();
            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var sale = await _context.SaleAccts.FindAsync(id);
            if (sale == null) return NotFound();

            _context.SaleAccts.Remove(sale);
            await _context.SaveChangesAsync();
            await _salesHub.Clients.All.SendAsync("SalesUpdated", "deleted", id);
            TempData["Success"] = "Sale record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}