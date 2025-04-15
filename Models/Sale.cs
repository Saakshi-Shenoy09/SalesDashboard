namespace SalesDashboard.Models
{
    public class Sale
    {
        public int SaleID { get; set; }
        public string ?ProductName { get; set; }
        public string ?Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
    }
}
