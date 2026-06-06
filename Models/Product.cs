namespace InventoryManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
        public decimal UnitPrice { get; set; }
        public string Description { get; set; }

        // Joined fields
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
        public bool IsLowStock => Quantity <= MinStockLevel;
    }
}