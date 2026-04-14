using Microsoft.AspNetCore.Mvc;
using SalesManagementSystem.Models;
using SalesManagementSystem.Repository;

public class SaleController : Controller
{
    private readonly ISaleRepository _repo;

    public SaleController(ISaleRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _repo.GetAll();
        return View(sales);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaleAcct saleacct)
    {
        await _repo.Create(saleacct);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(long id)
    {
        var saleacct = await _repo.GetById(id);
        return View(saleacct);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(SaleAcct saleacct)
    {
        await _repo.Update(saleacct);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(long id)
    {
        await _repo.Delete(id);
        return RedirectToAction("Index");
    }
}