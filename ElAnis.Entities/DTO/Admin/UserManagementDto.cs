namespace ElAnis.Entities.DTO.Admin
{
    public class UserManagementDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "user", "provider", "admin"
        public string Status { get; set; } = string.Empty; // "active", "inactive"
        public DateTime Joined { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class GetUsersRequest
    {
        public string? Search { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}