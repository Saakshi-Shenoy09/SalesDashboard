using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Models;
using System.Text.Json;

namespace SalesDashboard.Controllers
{
    public class SalesController : Controller
    {
        public string filePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "sales.json");
        public IActionResult Index(DateTime? fromDate, DateTime? toDate)
        {
            List<Sale> sales = LoadSales();

            if (fromDate.HasValue && toDate.HasValue)
            {
                sales = sales
                    .Where(s => s.SaleDate.Date >= fromDate && s.SaleDate.Date <= toDate) //this returns the query format of that data
                    .ToList(); //so this converts it into a list.
            }

            ViewBag.TotalOrders = sales.Count;
            ViewBag.TotalRevenue = sales.Sum(s => s.Amount);
            ViewBag.TotalSales = sales.Select(s => s.ProductName).Distinct().Count();

            ViewBag.MonthlyLabels = sales
                .GroupBy(s => s.SaleDate.ToString("MMM yyyy"))
                .Select(g => g.Key)
                .ToList();

            ViewBag.MonthlyData = sales
                .GroupBy(s => s.SaleDate.ToString("MMM yyyy"))
                .Select(g => g.Sum(x => x.Amount))
                .ToList();

            ViewBag.CategoryLabels = sales
                .GroupBy(s => s.Category) // Groups: Footwear -> [Boots, Sneakers], Accessories -> [Necklace]
                .Select(g => g.Key) // Takes Footwear & Accessories or from each group, take just the category name(the parameter based on which grouping was done).
                .ToList();

            ViewBag.CategoryData = sales
                .GroupBy(s => s.Category)
                .Select(g => g.Sum(x => x.Amount))
                .ToList();

            return View(sales);
        }

        public IActionResult ExportCsv()
        {
            List<Sale> sales = LoadSales();
            var rows = new List<string> { "SaleID,ProductName,Category,Amount,SaleDate" };
            rows.AddRange(sales.Select(s => $"{s.SaleID},{s.ProductName},{s.Category},{s.Amount},{s.SaleDate.ToString("yyyy-mm-dd")}"));
            var csv = string.Join("\n", rows);
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "Sales.csv");
        }

        private List<Sale> LoadSales()
        {
            if (!System.IO.File.Exists(filePath))
            {
                return new List<Sale>();
            }

            string json = System.IO.File.ReadAllText(filePath);
            List<Sale> ?saleList = JsonSerializer.Deserialize<List<Sale>>(json);
            if(saleList == null) //if deserialization fails.
            {
                saleList = new List<Sale>(); //return empty list so the app doeesnt crash
            }
            return saleList; //else return the list.
        }
    }
}
