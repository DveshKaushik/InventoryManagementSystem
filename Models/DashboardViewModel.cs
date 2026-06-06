namespace InventoryManagementSystem.Models
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalCategories { get; set; }
        public int LowStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public List<Product> LowStockProducts { get; set; }
        public List<StockChartData> ChartData { get; set; }
    }

    public class StockChartData
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int MinStockLevel { get; set; }
    }
}