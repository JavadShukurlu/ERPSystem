namespace ERPSystem.WebMVC.ViewModels.AuditLogs
{
    public class AuditLogViewModel
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string Action { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public int? EntityId { get; set; }

        public DateTime ActionDate { get; set; }

        public string? Details { get; set; }
    }
}