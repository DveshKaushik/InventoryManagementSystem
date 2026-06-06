using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
    public class SupplierController : Controller
    {
        private readonly DatabaseHelper _db;

        public SupplierController(DatabaseHelper db)
        {
            _db = db;
        }

        // LIST all suppliers
        public IActionResult Index()
        {
            var suppliers = new List<Supplier>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Suppliers ORDER BY Name", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    suppliers.Add(new Supplier
                    {
                        SupplierId = (int)reader["SupplierId"],
                        Name = reader["Name"].ToString(),
                        Contact = reader["Contact"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Email = reader["Email"].ToString(),
                        Address = reader["Address"].ToString()
                    });
            }
            return View(suppliers);
        }

        // SHOW add supplier form
        public IActionResult Create() => View();

        // SAVE new supplier
        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    INSERT INTO Suppliers 
                    (Name, Contact, Phone, Email, Address)
                    VALUES 
                    (@Name, @Contact, @Phone, @Email, @Address)", conn);
                cmd.Parameters.AddWithValue("@Name", supplier.Name);
                cmd.Parameters.AddWithValue("@Contact", supplier.Contact);
                cmd.Parameters.AddWithValue("@Phone", supplier.Phone);
                cmd.Parameters.AddWithValue("@Email", supplier.Email ?? "");
                cmd.Parameters.AddWithValue("@Address", supplier.Address ?? "");
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // DELETE supplier
        public IActionResult Delete(int id)
        {
            try
            {
                using (var conn = _db.GetConnection())
                {
                    conn.Open();

                    // Check if supplier has products linked
                    var checkCmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Products WHERE SupplierId = @Id", conn);
                    checkCmd.Parameters.AddWithValue("@Id", id);
                    var count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["Error"] = $"Cannot delete this supplier — " +
                            $"{count} product(s) are linked to it. " +
                            $"Delete or reassign those products first.";
                        return RedirectToAction("Index");
                    }

                    // Safe to delete
                    var cmd = new SqlCommand(
                        "DELETE FROM Suppliers WHERE SupplierId = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting supplier: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
    }
}