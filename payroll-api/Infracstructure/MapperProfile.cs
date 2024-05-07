using AutoMapper;
using Payroll.Data.Dtos.Users;
using Payroll.Logic.Model.User;

namespace payroll_api.Infracstructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserInfoDto, UserInfoModel>().ReverseMap();
        }
    }
}
