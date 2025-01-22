using AutoMapper;
using CleanArchitecture.Application.Restaurants.Dtos;
using CleanArchitecture.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Restaurants;

internal class RestaurantsService(IRestaurantsRepository restaurantsRepository, ILogger<RestaurantsService> logger, IMapper mapper) : IRestaurantsService
{
    public async Task<IEnumerable<RestaurantDto>> GetAllRestaurants()
    {
        logger.LogInformation("获取所有餐馆");
        var restaurants = await restaurantsRepository.GetAllAsync();
        //使用DTO
        //var restaurantsDto = restaurants.Select(RestaurantDto.FromEntity);
        //使用AutoMapper
        var restaurantsDto = mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
        return restaurantsDto!;
    }

    public async Task<RestaurantDto?> GetId(int id)
    {
        logger.LogInformation($"{id}");
        var restaurant = await restaurantsRepository.GetIdAsync(id);
        //var restaurantDto = RestaurantDto.FromEntity(restaurant);
        var restaurantDto = mapper.Map<RestaurantDto?>(restaurant);
        return restaurantDto;
    }
}
