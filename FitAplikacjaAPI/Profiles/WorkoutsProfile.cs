using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Workouts;
using FitAplikacja.Core.Dtos.Output.Workouts;
using FitAplikacja.Core.Models;

namespace FitAplikacjaAPI.Profiles
{
    public class WorkoutsProfile : Profile
    {
        public WorkoutsProfile()
        {
            CreateMap<WorkoutInputRequest, Workout>();

            CreateMap<Workout, WorkoutResponse>()
                .ForMember(x => x.Id, d => d.MapFrom(s => s.Id))
                .ForMember(x => x.Completed, d => d.MapFrom(s => s.Completed))
                .ForMember(x => x.ApplicationUserId, d => d.MapFrom(s => s.ApplicationUserId))
                .ForMember(x => x.Exercises, d => d.MapFrom(s => s.Exercises));
        }
    }
}
