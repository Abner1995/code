using CleanArchitecture.Application.Restaurants.Dtos;

namespace CleanArchitecture.Application.Restaurants;

public interface IRestaurantsService
{
    Task<IEnumerable<RestaurantDto>> GetAllRestaurants();
    Task<RestaurantDto?> GetId(int id);
    Task<int> Create(CreateRestaurantDto createRestaurantDto);
}
