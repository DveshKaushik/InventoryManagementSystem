namespace InventoryManagementSystem.Models
{
    public class StockMovement
    {
        public int MovementId { get; set; }
        public int ProductId { get; set; }
        public string MovementType { get; set; }
        public int Quantity { get; set; }
        public DateTime MovementDate { get; set; }
        public string Reason { get; set; }

        // Joined field
        public string ProductName { get; set; }
    }
}