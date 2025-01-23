using AutoMapper;
using CleanArchitecture.Application.Restaurants.Commands.CreateRestaurant;
using CleanArchitecture.Application.Restaurants.Commands.UpdateRestaurant;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Restaurants.Dtos;

public class RestaurantProfile : Profile
{
    public RestaurantProfile()
    {
        CreateMap<UpdateRestaurantCommand, Restaurant>();

        //使用MediatR把CreateRestaurantDto改成CreateRestaurantCommand
        CreateMap<CreateRestaurantCommand, Restaurant>()
            .ForMember(d => d.Address, opt => opt.MapFrom(src => new Address()
            {
                City = src.City,
                Street = src.Street,
                PostalCode = src.PostalCode,
            }));

        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(d => d.City, opt => opt.MapFrom(src => src == null ? null : src.Address.City))
            .ForMember(d => d.Street, opt => opt.MapFrom(src => src == null ? null : src.Address.Street))
            .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src == null ? null : src.Address.PostalCode))
            .ForMember(d => d.Dishes, opt => opt.MapFrom(src => src.Dishes));
    }
}
