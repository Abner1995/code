using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.Restaurants;

internal class RestaurantsService(IRestaurantsRepository restaurantsRepository, ILogger<RestaurantsService> logger) : IRestaurantsService
{
    public async Task<IEnumerable<Restaurant>> GetAllRestaurants()
    {
        logger.LogInformation("获取所有餐馆");
        var restaurants = await restaurantsRepository.GetAllAsync();
        return restaurants;
    }

    public async Task<Restaurant?> GetId(int id)
    {
        logger.LogInformation($"{id}");
        var restaurant = await restaurantsRepository.GetIdAsync(id);
        return restaurant;
    }
}
