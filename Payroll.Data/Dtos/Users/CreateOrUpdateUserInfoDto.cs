namespace Payroll.Data.Dtos.Users
{
    public class CreateOrUpdateUserInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Description { get; set; }
        public int? TokenLifetimeMinutes { get; set; }
        public string Email { get; set; }
        public string[] RoleNames { get; set; }
    }
}
