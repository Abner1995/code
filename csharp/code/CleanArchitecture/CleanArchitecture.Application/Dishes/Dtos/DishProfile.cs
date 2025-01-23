using AutoMapper;
using CleanArchitecture.Application.Dishes.Commands.CreateDish;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Dishes.Dtos;

public class DishProfile : Profile
{
    public DishProfile()
    {
        CreateMap<CreateDishCommand, Dish>();
        CreateMap<Dish, DishDto>();
    }
}
