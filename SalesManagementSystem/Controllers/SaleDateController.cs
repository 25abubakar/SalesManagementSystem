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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SaleDate saleDate)
        {
            if (ModelState.IsValid)
            {
                var currentDate = DateTime.Now;

                saleDate.TransDate = currentDate;
                saleDate.ProcessDate = currentDate;
                saleDate.OrderDate = currentDate;
                saleDate.SoldDate = currentDate;
                saleDate.PaymentDate = currentDate;

                _context.SaleDates.Add(saleDate);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(saleDate);
        }

        public IActionResult Edit(int id)
        {
            var data = _context.SaleDates.Find(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SaleDate saleDate)
        {
            if (ModelState.IsValid)
            {
                _context.SaleDates.Update(saleDate);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(saleDate);
        }

        public IActionResult Delete(int id)
        {
            var data = _context.SaleDates.Find(id);

            if (data == null)
                return NotFound();

            return View(data);
        }

 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var data = _context.SaleDates.Find(id);

            if (data != null)
            {
                _context.SaleDates.Remove(data);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}