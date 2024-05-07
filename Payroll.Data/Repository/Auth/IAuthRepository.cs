using Payroll.Data.Dtos.Users;

namespace Payroll.Data.Repository.Auth
{
    public interface IAuthRepository
    {
        ValueTask<UserInfoDto> GetByUsername(string userName);
    }
}
