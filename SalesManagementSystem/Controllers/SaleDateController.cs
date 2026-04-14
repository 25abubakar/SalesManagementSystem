using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var data = _context.SaleDates.ToList();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SaleAcct saleAcct)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateTime.Now;

                var saleDate = new SaleDate
                {
                    TransDate = currentDate,
                    ProcessDate = currentDate,
                    OrderDate = currentDate,
                    SoldDate = currentDate,
                    PaymentDate = currentDate
                };

                _context.SaleDates.Add(saleDate);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(saleAcct);
        }
    }
}