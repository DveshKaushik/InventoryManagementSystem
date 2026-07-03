using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class ProductController : Controller
    {
        private readonly DatabaseHelper _db;

        public ProductController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all products with category and supplier names
        public IActionResult Index()
        {
            var products = new List<Product>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    SELECT p.*, c.Name AS CategoryName, s.Name AS SupplierName
                    FROM Products p
                    INNER JOIN Categories c ON p.CategoryId = c.CategoryId
                    INNER JOIN Suppliers s ON p.SupplierId = s.SupplierId
                    ORDER BY p.Quantity ASC", conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = reader["Name"].ToString(),
                        CategoryId = (int)reader["CategoryId"],
                        SupplierId = (int)reader["SupplierId"],
                        Quantity = (int)reader["Quantity"],
                        MinStockLevel = (int)reader["MinStockLevel"],
                        UnitPrice = (decimal)reader["UnitPrice"],
                        Description = reader["Description"].ToString(),
                        CategoryName = reader["CategoryName"].ToString(),
                        SupplierName = reader["SupplierName"].ToString()
                    });
                }
            }
            return View(products);
        }

        // SHOW add product form
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // SAVE new product
        [HttpPost]
        public IActionResult Create(Product product)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(@"
                    INSERT INTO Products 
                    (Name, CategoryId, SupplierId, Quantity, MinStockLevel, UnitPrice, Description)
                    VALUES 
                    (@Name, @CategoryId, @SupplierId, @Quantity, @MinStockLevel, @UnitPrice, @Description)",
                    conn);
                cmd.Parameters.AddWithValue("@Name", product.Name);
                cmd.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                cmd.Parameters.AddWithValue("@SupplierId", product.SupplierId);
                cmd.Parameters.AddWithValue("@Quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@MinStockLevel", product.MinStockLevel);
                cmd.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                cmd.Parameters.AddWithValue("@Description", product.Description ?? "");
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // DELETE product
        public IActionResult Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                // Delete stock movements first
                var cmd1 = new MySqlCommand(
                    "DELETE FROM StockMovements WHERE ProductId = @Id", conn);
                cmd1.Parameters.AddWithValue("@Id", id);
                cmd1.ExecuteNonQuery();

                // Then delete product
                var cmd2 = new MySqlCommand(
                    "DELETE FROM Products WHERE ProductId = @Id", conn);
                cmd2.Parameters.AddWithValue("@Id", id);
                cmd2.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // ADD STOCK
        public IActionResult AddStock(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT * FROM Products WHERE ProductId = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ViewBag.Product = new Product
                    {
                        ProductId = (int)reader["ProductId"],
                        Name = reader["Name"].ToString(),
                        Quantity = (int)reader["Quantity"]
                    };
                }
            }
            return View();
        }

        [HttpPost]
        public IActionResult AddStock(int productId, int quantity, string reason)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Update product quantity
                var cmd1 = new MySqlCommand(@"
                    UPDATE Products 
                    SET Quantity = Quantity + @Quantity 
                    WHERE ProductId = @Id", conn);
                cmd1.Parameters.AddWithValue("@Quantity", quantity);
                cmd1.Parameters.AddWithValue("@Id", productId);
                cmd1.ExecuteNonQuery();

                // Record stock movement
                var cmd2 = new MySqlCommand(@"
                    INSERT INTO StockMovements 
                    (ProductId, MovementType, Quantity, Reason)
                    VALUES (@ProductId, 'IN', @Quantity, @Reason)", conn);
                cmd2.Parameters.AddWithValue("@ProductId", productId);
                cmd2.Parameters.AddWithValue("@Quantity", quantity);
                cmd2.Parameters.AddWithValue("@Reason", reason ?? "Stock added");
                cmd2.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // Helper — load category and supplier dropdowns
        private void LoadDropdowns()
        {
            var categories = new List<Category>();
            var suppliers = new List<Supplier>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                var catCmd = new MySqlCommand(
                    "SELECT CategoryId, Name FROM Categories", conn);
                var catReader = catCmd.ExecuteReader();
                while (catReader.Read())
                    categories.Add(new Category
                    {
                        CategoryId = (int)catReader["CategoryId"],
                        Name = catReader["Name"].ToString()
                    });
                catReader.Close();

                var supCmd = new MySqlCommand(
                    "SELECT SupplierId, Name FROM Suppliers", conn);
                var supReader = supCmd.ExecuteReader();
                while (supReader.Read())
                    suppliers.Add(new Supplier
                    {
                        SupplierId = (int)supReader["SupplierId"],
                        Name = supReader["Name"].ToString()
                    });
                supReader.Close();
            }

            ViewBag.Categories = categories;
            ViewBag.Suppliers = suppliers;
        }
    }
}