using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Restaurants;

public interface IRestaurantsService
{
    Task<IEnumerable<Restaurant>> GetAllRestaurants();
    Task<Restaurant?> GetId(int id);
}
