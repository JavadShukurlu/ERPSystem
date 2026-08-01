namespace ERPSystem.Application.DTOs.Users
{
    public class UserDto
    {
        public string Id { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}