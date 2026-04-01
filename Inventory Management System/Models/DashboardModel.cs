namespace Inventory_Management_System.Models
{
    public class DashboardModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int LowStock { get; set; }
    }
}
