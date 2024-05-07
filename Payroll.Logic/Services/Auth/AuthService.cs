using AutoMapper;
using Payroll.Data.Dtos.Users;
using Payroll.Data.Repository.Auth;
using Payroll.Logic.Model.User;

namespace Payroll.Logic.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly Lazy<IAuthRepository> _authRepository;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IMapper mapper)
        {
            _authRepository = new Lazy<IAuthRepository>(() => authRepository);
            _mapper = mapper;
        }

        public async ValueTask<UserInfoModel> GetByUsername(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) return null;
                UserInfoDto user = await _authRepository.Value.GetByUsername(userName);
                if (user == null) return null;
                UserInfoModel result = _mapper.Map<UserInfoModel>(user);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
