namespace Inventory_Management_System.Models
{
    public class InventoryModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public int CategoryId { get; set; }

        public int SupplierId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }

      
        public string? CategoryName { get; set; }

        public string? SupplierName { get; set; }
    }
}
