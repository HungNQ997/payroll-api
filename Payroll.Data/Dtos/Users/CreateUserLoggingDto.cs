namespace Payroll.Data.Dtos.Users
{
    public class CreateUserLoggingDto
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Username { get; set; }
    }
}
