using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesManagementSystem.Data;
using SalesManagementSystem.Models;

namespace SalesManagementSystem.Controllers
{
    public class SaleDateController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleDateController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.SaleDates.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleDate saleDate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(saleDate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(saleDate);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _context.SaleDates.FindAsync(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaleDate saleDate)
        {
            _context.Update(saleDate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}