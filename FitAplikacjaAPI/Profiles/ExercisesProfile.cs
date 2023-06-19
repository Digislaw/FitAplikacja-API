using AutoMapper;
using FitAplikacja.Core.Dtos.Input.Exercises;
using FitAplikacja.Core.Dtos.Output.Exercises;
using FitAplikacja.Core.Models;

namespace FitAplikacjaAPI.Profiles
{
    public class ExercisesProfile : Profile
    {
        public ExercisesProfile()
        {
            CreateMap<ExerciseRequest, Exercise>();
            CreateMap<Exercise, ExerciseResponse>();
        }
    }
}
