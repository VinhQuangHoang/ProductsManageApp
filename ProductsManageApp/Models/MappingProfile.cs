using AutoMapper;

namespace ProductsManageApp.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.PhoneNumber, opt => opt.MapFrom(x => x.PhoneNumber))
                .ForMember(u => u.FullName, opt => opt.MapFrom(x => x.UserName))
                /*.ForMember(u => u.PasswordHash, opt => opt.MapFrom(x => x.Password))*/;

            //CreateMap<ExternalLogin, User>()
            //    .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email))
            //    .ForMember(u => u.HoTen, opt => opt.MapFrom(x => x.Principal.FindFirst(ClaimTypes.Name).Value));

        }


    }
}
