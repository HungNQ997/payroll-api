using Payroll.Logic.Model.User;

namespace Payroll.Logic.Services.Auth
{
    public interface IAuthService
    {
        ValueTask<UserInfoModel> GetByUsername(string username);
    }
}
