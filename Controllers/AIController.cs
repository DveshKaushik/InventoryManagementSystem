using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class AIController : Controller
    {
        private readonly GroqService _groq;
        private readonly DatabaseHelper _db;

        public AIController(GroqService groq, DatabaseHelper db)
        {
            _groq = groq;
            _db = db;
        }

        // ─── Stock Analyzer ────────────────────────────────────────

        public IActionResult StockAnalyzer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StockAnalyzer(string question)
        {
            // Fetch all inventory data
            var products = new List<string>();
            var lowStock = new List<string>();
            var movements = new List<string>();
            var totalValue = 0m;
            var totalProducts = 0;
            var lowStockCount = 0;

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Get all products with status
                var prodCmd = new SqlCommand(@"
                    SELECT p.Name, p.Quantity, p.MinStockLevel,
                           p.UnitPrice, c.Name AS Category,
                           s.Name AS Supplier
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                    INNER JOIN Suppliers s ON p.SupplierId = s.SupplierId",
                    conn);

                var prodReader = prodCmd.ExecuteReader();
                while (prodReader.Read())
                {
                    var qty = (int)prodReader["Quantity"];
                    var minLevel = (int)prodReader["MinStockLevel"];
                    var price = (decimal)prodReader["UnitPrice"];
                    var name = prodReader["Name"].ToString();
                    var status = qty <= minLevel ? "LOW STOCK" : "OK";

                    products.Add($"{name} | Category: {prodReader["Category"]} | " +
                        $"Supplier: {prodReader["Supplier"]} | " +
                        $"Qty: {qty} | Min: {minLevel} | " +
                        $"Price: ₹{price} | Status: {status}");

                    totalValue += qty * price;
                    totalProducts++;

                    if (qty <= minLevel)
                    {
                        lowStockCount++;
                        lowStock.Add($"{name} (Current: {qty}, Minimum: {minLevel})");
                    }
                }
                prodReader.Close();

                // Get recent stock movements
                var movCmd = new SqlCommand(@"
                    SELECT TOP 10 p.Name, sm.MovementType,
                           sm.Quantity, sm.MovementDate, sm.Reason
                    FROM StockMovements sm
                    INNER JOIN Products p ON sm.ProductId = p.ProductId
                    ORDER BY sm.MovementDate DESC", conn);

                var movReader = movCmd.ExecuteReader();
                while (movReader.Read())
                    movements.Add(
                        $"{movReader["Name"]} | {movReader["MovementType"]} | " +
                        $"Qty: {movReader["Quantity"]} | " +
                        $"Date: {Convert.ToDateTime(movReader["MovementDate"]).ToString("dd MMM yyyy")} | " +
                        $"Reason: {movReader["Reason"]}");
                movReader.Close();
            }

            // Build AI context
            var context = $@"
                You are an intelligent inventory management assistant 
                for a manufacturing/petrochemical company.

                CURRENT INVENTORY STATUS:
                Total Products: {totalProducts}
                Low Stock Items: {lowStockCount}
                Total Inventory Value: ₹{totalValue:N0}

                PRODUCT LIST:
                {string.Join("\n", products)}

                LOW STOCK ITEMS:
                {(lowStock.Count > 0 ? string.Join("\n", lowStock) : "None")}

                RECENT STOCK MOVEMENTS (Last 10):
                {string.Join("\n", movements)}

                Based on this inventory data, answer the following question
                with specific insights and actionable recommendations.
                Be concise and professional.

                Question: {question}";

            var response = await _groq.GetResponseAsync(context);

            ViewBag.Question = question;
            ViewBag.Response = response;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.TotalValue = totalValue;
            ViewBag.TotalProducts = totalProducts;

            return View();
        }

        // ─── AI Chatbot ────────────────────────────────────────────

        public IActionResult Chatbot()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chatbot(string message)
        {
            var products = new List<string>();
            var suppliers = new List<string>();
            var movements = new List<string>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Products
                var prodCmd = new SqlCommand(@"
                    SELECT p.Name, p.Quantity, p.MinStockLevel,
                           p.UnitPrice, c.Name AS Category
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.CategoryId",
                    conn);
                var prodReader = prodCmd.ExecuteReader();
                while (prodReader.Read())
                    products.Add(
                        $"{prodReader["Name"]} | Qty: {prodReader["Quantity"]} | " +
                        $"Min: {prodReader["MinStockLevel"]} | " +
                        $"Category: {prodReader["Category"]}");
                prodReader.Close();

                // Suppliers
                var supCmd = new SqlCommand(
                    "SELECT Name, Contact, Phone FROM Suppliers", conn);
                var supReader = supCmd.ExecuteReader();
                while (supReader.Read())
                    suppliers.Add(
                        $"{supReader["Name"]} | Contact: {supReader["Contact"]} | " +
                        $"Phone: {supReader["Phone"]}");
                supReader.Close();

                // Recent movements
                var movCmd = new SqlCommand(@"
                    SELECT TOP 5 p.Name, sm.MovementType, sm.Quantity
                    FROM StockMovements sm
                    INNER JOIN Products p ON sm.ProductId = p.ProductId
                    ORDER BY sm.MovementDate DESC", conn);
                var movReader = movCmd.ExecuteReader();
                while (movReader.Read())
                    movements.Add(
                        $"{movReader["Name"]} | {movReader["MovementType"]} | " +
                        $"Qty: {movReader["Quantity"]}");
                movReader.Close();
            }

            var context = $@"
                You are a helpful inventory assistant.
                Answer questions about the inventory data below.
                Be concise and helpful.

                Products: {string.Join(", ", products)}
                Suppliers: {string.Join(", ", suppliers)}
                Recent Movements: {string.Join(", ", movements)}

                Question: {message}";

            var response = await _groq.GetResponseAsync(context);

            ViewBag.Message = message;
            ViewBag.Response = response;

            return View();
        }
    }
}