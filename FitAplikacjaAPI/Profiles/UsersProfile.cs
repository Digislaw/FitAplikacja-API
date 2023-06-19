using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Users;
using FitAplikacja.Core.Dtos.Output.Users;
using FitAplikacja.Core.Models;

namespace FitAplikacjaAPI.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserRegistrationRequest, ApplicationUser>()
                .ForMember(x => x.UserName, d => d.MapFrom(s => s.Username))
                .ForMember(x => x.Email, d => d.MapFrom(s => s.Email))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<ApplicationUserDetailsRequest, ApplicationUser>();

            // data for admin
            CreateMap<ApplicationUser, ApplicationUserResponse>();

            // data for user
            CreateMap<ApplicationUser, ApplicationUserDetailsResponse>();
        }
    }
}
