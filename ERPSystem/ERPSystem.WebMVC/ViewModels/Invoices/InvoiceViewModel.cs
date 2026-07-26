namespace ERPSystem.WebMVC.ViewModels.Invoices
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public int Status { get; set; }

        public int SaleId { get; set; }

        public string? CustomerName { get; set; }
    }
}
