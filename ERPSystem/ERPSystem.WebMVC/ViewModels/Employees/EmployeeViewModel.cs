namespace ERPSystem.WebMVC.ViewModels.Employees
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Position { get; set; }

        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public string? ImageUrl { get; set; }

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }
    }
}