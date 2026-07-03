using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseHelper _db;

        public HomeController(DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel();
            model.LowStockProducts = new List<Product>();
            model.ChartData = new List<StockChartData>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Total products
                model.TotalProducts = Convert.ToInt32(new MySqlCommand(
                    "SELECT COUNT(*) FROM Products", conn).ExecuteScalar());

                // Total suppliers
                model.TotalSuppliers = Convert.ToInt32(new MySqlCommand(
                    "SELECT COUNT(*) FROM Suppliers", conn).ExecuteScalar());

                // Total categories
                model.TotalCategories = Convert.ToInt32(new MySqlCommand(
                    "SELECT COUNT(*) FROM Categories", conn).ExecuteScalar());

                // Low stock count
                model.LowStockCount = Convert.ToInt32(new MySqlCommand(
                    "SELECT COUNT(*) FROM Products WHERE Quantity <= MinStockLevel",
                    conn).ExecuteScalar());

                // Total inventory value
                model.TotalInventoryValue = Convert.ToDecimal(new MySqlCommand(
                    "SELECT COALESCE(SUM(Quantity * UnitPrice), 0) FROM Products",
                    conn).ExecuteScalar());

                // Low stock products
                var lowCmd = new MySqlCommand(@"
                    SELECT p.*, c.Name AS CategoryName, s.Name AS SupplierName
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                    INNER JOIN Suppliers s ON p.SupplierId = s.SupplierId
                    WHERE p.Quantity <= p.MinStockLevel
                    ORDER BY p.Quantity ASC", conn);

                var lowReader = lowCmd.ExecuteReader();
                while (lowReader.Read())
                    model.LowStockProducts.Add(new Product
                    {
                        ProductId = (int)lowReader["ProductId"],
                        Name = lowReader["Name"].ToString(),
                        Quantity = (int)lowReader["Quantity"],
                        MinStockLevel = (int)lowReader["MinStockLevel"],
                        CategoryName = lowReader["CategoryName"].ToString(),
                        SupplierName = lowReader["SupplierName"].ToString()
                    });
                lowReader.Close();

                // Chart data - top 6 products stock levels
                var chartCmd = new MySqlCommand(@"
                    SELECT Name, Quantity, MinStockLevel
                    FROM Products
                    ORDER BY Quantity ASC
                    LIMIT 6", conn);

                var chartReader = chartCmd.ExecuteReader();
                while (chartReader.Read())
                    model.ChartData.Add(new StockChartData
                    {
                        ProductName = chartReader["Name"].ToString(),
                        Quantity = (int)chartReader["Quantity"],
                        MinStockLevel = (int)chartReader["MinStockLevel"]
                    });
                chartReader.Close();
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}