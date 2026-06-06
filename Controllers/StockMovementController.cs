using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class StockMovementController : Controller
    {
        private readonly DatabaseHelper _db;

        public StockMovementController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all stock movements
        public IActionResult Index()
        {
            var movements = new List<StockMovement>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT sm.*, p.Name AS ProductName
                    FROM StockMovements sm
                    INNER JOIN Products p ON sm.ProductId = p.ProductId
                    ORDER BY sm.MovementDate DESC", conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    movements.Add(new StockMovement
                    {
                        MovementId = (int)reader["MovementId"],
                        ProductId = (int)reader["ProductId"],
                        ProductName = reader["ProductName"].ToString(),
                        MovementType = reader["MovementType"].ToString(),
                        Quantity = (int)reader["Quantity"],
                        MovementDate = (DateTime)reader["MovementDate"],
                        Reason = reader["Reason"].ToString()
                    });
            }
            return View(movements);
        }

        // SHOW add movement form
        public IActionResult Create()
        {
            LoadProducts();
            return View();
        }

        // SAVE new stock movement
        [HttpPost]
        public IActionResult Create(StockMovement movement)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Check if OUT movement has enough stock
                if (movement.MovementType == "OUT")
                {
                    var checkCmd = new SqlCommand(
                        "SELECT Quantity FROM Products WHERE ProductId = @Id",
                        conn);
                    checkCmd.Parameters.AddWithValue("@Id", movement.ProductId);
                    var currentQty = (int)checkCmd.ExecuteScalar();

                    if (currentQty < movement.Quantity)
                    {
                        TempData["Error"] =
                            $"Not enough stock! Current quantity is {currentQty} units.";
                        LoadProducts();
                        return View(movement);
                    }
                }

                // Update product quantity
                var updateSql = movement.MovementType == "IN"
                    ? "UPDATE Products SET Quantity = Quantity + @Qty WHERE ProductId = @Id"
                    : "UPDATE Products SET Quantity = Quantity - @Qty WHERE ProductId = @Id";

                var updateCmd = new SqlCommand(updateSql, conn);
                updateCmd.Parameters.AddWithValue("@Qty", movement.Quantity);
                updateCmd.Parameters.AddWithValue("@Id", movement.ProductId);
                updateCmd.ExecuteNonQuery();

                // Record the movement
                var insertCmd = new SqlCommand(@"
                    INSERT INTO StockMovements
                    (ProductId, MovementType, Quantity, Reason)
                    VALUES
                    (@ProductId, @MovementType, @Quantity, @Reason)", conn);
                insertCmd.Parameters.AddWithValue("@ProductId", movement.ProductId);
                insertCmd.Parameters.AddWithValue("@MovementType", movement.MovementType);
                insertCmd.Parameters.AddWithValue("@Quantity", movement.Quantity);
                insertCmd.Parameters.AddWithValue("@Reason", movement.Reason ?? "");
                insertCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // Helper — load products dropdown
        private void LoadProducts()
        {
            var products = new List<Product>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ProductId, Name, Quantity FROM Products ORDER BY Name",
                    conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    products.Add(new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = reader["Name"].ToString(),
                        Quantity = (int)reader["Quantity"]
                    });
            }
            ViewBag.Products = products;
        }
    }
}